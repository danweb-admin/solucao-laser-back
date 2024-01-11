using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Exceptions.Model;
using Solucao.Application.Service.Interfaces;
using Calendar = Solucao.Application.Data.Entities.Calendar;

namespace Solucao.Application.Service.Implementations
{
	public class GenerateContractService : IGenerateContractService
	{
        private readonly IMapper mapper;
        private readonly CalendarRepository calendarRepository;
        private readonly ModelRepository modelRepository;
        private CultureInfo cultureInfo = new CultureInfo("pt-BR");


        public GenerateContractService(IMapper _mapper, CalendarRepository _calendarRepository, ModelRepository _modelRepository)
		{
            mapper = _mapper;
            calendarRepository = _calendarRepository;
            modelRepository = _modelRepository;
		}

        public async Task<IEnumerable<CalendarViewModel>> GetAllByDayAndContractMade(DateTime date)
        {
            return mapper.Map<IEnumerable<CalendarViewModel>>(await calendarRepository.GetAllByDayAndConfirmed(date));
        }

        public async Task<byte[]> DownloadContract(Guid calendarId)
        {

            var calendar = await calendarRepository.GetById(calendarId);

            if (!File.Exists(calendar.ContractPath))
                throw new ContractNotFoundException("Contrato não encontrado.");

            return await File.ReadAllBytesAsync(calendar.ContractPath);

        }

        public async Task<ValidationResult> GenerateContract(GenerateContractRequest request)
        {
            var modelPath = Environment.GetEnvironmentVariable("ModelDocsPath");
            var contractPath = Environment.GetEnvironmentVariable("DocsPath");

            var calendar = await calendarRepository.GetById(request.CalendarId);

            var model = await modelRepository.GetByEquipament(calendar.EquipamentId);

            if (model == null)
                throw new ModelNotFoundException("Modelo de contrato para esse equipamento não encontrado.");

            var contractFileName = FormatNameFile(calendar.Client.Name, calendar.Equipament.Name, calendar.Date);

            var copiedFile = await CopyFileStream(modelPath, contractPath,model.ModelFileName, contractFileName, calendar.Date);

            var result = ExecuteReplace(copiedFile, model, calendar);

            if (result)
            {
                calendar.ContractPath = copiedFile;
                calendar.UpdatedAt = DateTime.Now;
                calendar.ContractMade = true;
                await calendarRepository.Update(calendar);

                return ValidationResult.Success;
            }

            return new ValidationResult("Erro para gerar o contrato");
        }

        

        private string FormatNameFile(string locatarioName, string equipamentName, DateTime date)
        {
            var _locatarioName = locatarioName.Replace(" ","");
            var _equipamentName = equipamentName.Replace(" ", "");
            var _date = date.ToString("dd-MM-yyyy");

            return $"{_locatarioName}-{_equipamentName}-{_date}.docx";
        }

        private async Task<string> CopyFileStream(string modelDirectory, string contractDirectory, string modelFileName, string fileName, DateTime date)
        {
            FileInfo inputFile = new FileInfo(modelDirectory + modelFileName);

            var yearMonth = date.ToString("yyyy-MM");
            var day = date.ToString("dd");

            var createdDirectory = $"{contractDirectory}/{yearMonth}/{day}";

            using (FileStream originalFileStream = inputFile.OpenRead())
            {
                if (!Directory.Exists(createdDirectory))
                    Directory.CreateDirectory(createdDirectory);

                var outputFileName = Path.Combine(createdDirectory, fileName);
                using (FileStream outputFileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    await originalFileStream.CopyToAsync(outputFileStream);
                }
                return outputFileName;
            }
        }

        private bool ExecuteReplace(string copiedFile, Model model, Calendar calendar)
        {
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(copiedFile, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                        docText = sr.ReadToEnd();

                    foreach (var item in model.ModelAttributes)
                    {
                        Regex regexText = new Regex(item.FileAttribute.Trim());
                        var valueItem = GetPropertieValue(calendar, item.TechnicalAttribute, item.AttributeType);
                        docText = regexText.Replace(docText, valueItem);
                    }

                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                        sw.Write(docText);

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            
        }

        private string GetPropertieValue(object obj, string propertieName, string attrType)
        {
            // Dividir o nome da propriedade para acessar propriedades aninhadas
            string[] properties = propertieName.Split('.');

            object value = obj;

            // Iterar sobre as propriedades
            foreach (var prop in properties)
            {
                // Obter tipo do objeto atual
                Type type = value.GetType();

                // Obter propriedade pelo nome
                var propInfo = type.GetProperty(prop);

                // Se a propriedade não existir, retornar null
                if (propInfo == null)
                {
                    return null;
                }

                // Obter valor da propriedade
                value = propInfo.GetValue(value);
            }

            // Converter valor para string (assumindo que a propriedade é do tipo string)
            return FormatValue(value.ToString(), attrType);
        }

        private string FormatValue(string value, string attrType)
        {
            switch (attrType)
            {
                case "datetime":
                    return DateTime.Parse(value).ToString("dd/MM/yyyy");
                case "datetime_extenso":
                    return DateTime.Parse(value).ToString("D", cultureInfo);
                case "time":
                    return DateTime.Parse(value).ToString("HH:mm");
                case "decimal":
                    return decimal.Parse(value).ToString().Replace(".", ",");
                case "decimal_extenso":
                    var decimalSplit = Decimal.Parse(value).ToString("n2").Split('.');
                    var part1 = long.Parse(decimalSplit[0].Replace(",","")).ToWords(cultureInfo);
                    var part2 = int.Parse(decimalSplit[1]).ToWords(cultureInfo);

                    if (part2 == "zero")
                        return $"{part1} reais";
                    return $"{part1} reais e {part2} centavos";
                default:
                    return value;
            }
        }

        
    }
}

