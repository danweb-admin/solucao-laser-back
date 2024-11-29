﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using Humanizer;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Exceptions.Model;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;
using Calendar = Solucao.Application.Data.Entities.Calendar;


namespace Solucao.Application.Service.Implementations
{
    public class GenerateContractService : IGenerateContractService
    {
        private readonly IMapper mapper;
        private readonly CalendarRepository calendarRepository;
        private readonly ModelRepository modelRepository;
        private readonly ModelAttributesRepository modelAttributesRepository;
        private readonly IClientRepository clientRepository;
        private CultureInfo cultureInfo = new CultureInfo("pt-BR");


        public GenerateContractService(IMapper _mapper, CalendarRepository _calendarRepository, ModelRepository _modelRepository, IClientRepository _clientRepository, ModelAttributesRepository _modelAttributesRepository)
        {
            mapper = _mapper;
            calendarRepository = _calendarRepository;
            modelRepository = _modelRepository;
            clientRepository = _clientRepository;
            modelAttributesRepository = _modelAttributesRepository;
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

            var calendar = mapper.Map<CalendarViewModel>(await calendarRepository.GetById(request.CalendarId));
            calendar.RentalTime = CalculateMinutes(calendar.StartTime.Value, calendar.EndTime.Value);
            await SearchCustomerValue(calendar);
            calendar.TotalValue = calendar.Value + calendar.Freight - calendar.Discount + calendar.Additional1;

            var model = await modelRepository.GetByEquipament(calendar.EquipamentId);

            if (model == null)
                throw new ModelNotFoundException("Modelo de contrato para esse equipamento não encontrado.");

            var attributes = await modelAttributesRepository.GetAll();

            var contractFileName = FormatNameFile(calendar.Client.Name, calendar.Equipament.Name, calendar.Date);

            var copiedFile = await CopyFileStream(modelPath, contractPath, model.ModelFileName, contractFileName, calendar.Date);

            var result = ExecuteReplace(copiedFile, attributes, calendar);

            if (result)
            {
                calendar.ContractPath = copiedFile;
                calendar.UpdatedAt = DateTime.Now;
                calendar.ContractMade = true;

                await calendarRepository.Update(mapper.Map<Calendar>(calendar));

                return ValidationResult.Success;
            }

            return new ValidationResult("Erro para gerar o contrato");
        }

        private string FormatNameFile(string locatarioName, string equipamentName, DateTime date)
        {
            var _locatarioName = locatarioName.Replace(" ", "");
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

        private bool ExecuteReplace(string copiedFile, IEnumerable<ModelAttributes> attributes, CalendarViewModel calendar)
        {
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(copiedFile, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                        docText = sr.ReadToEnd();

                    foreach (var item in attributes)
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
                    var monthDay = DateTime.Parse(value).ToString("M", cultureInfo);
                    var year = DateTime.Parse(value).ToString("yyyy", cultureInfo);

                    return $"{monthDay} de {year}";
                case "time":
                    return DateTime.Parse(value).ToString("HH:mm");
                case "decimal":
                    return decimal.Parse(value).ToString("N2", cultureInfo);
                case "decimal_extenso":
                    return decimalExtenso(value);
                case "time_extenso":
                    return timeExtenso(value);
                default:
                    return value;
            }
        }

        private string decimalExtenso(string value)
        {
            var decimalSplit = decimal.Parse(value).ToString("n2").Split('.');
            var part1 = long.Parse(decimalSplit[0].Replace(",", "")).ToWords(cultureInfo).ToTitleCase(TitleCase.First);
            var part2 = int.Parse(decimalSplit[1]).ToWords(cultureInfo);

            if (part2 == "zero")
                return $"{part1} reais";
            return $"{part1} reais e {part2} centavos";
        }

        private string timeExtenso(string value)
        {
            var minutesTotal = int.Parse(value);

            int hours = minutesTotal / 60;
            int minutes = minutesTotal % 60;

            string result = "";

            if (hours == 0)
            {
                if (minutes > 0)
                    result += $"{minutes} {(minutes == 1 ? "minuto" : "minutos")}";
                return result;
            }

            result = $"{hours} {(hours == 1 ? "hora" : "horas")}";

            if (minutes > 0)
                result += $" e {minutes} {(minutes == 1 ? "minuto" : "minutos")}";

            return result;
        }

        private async Task SearchCustomerValue(CalendarViewModel calendar)
        {
            TimeSpan difference = calendar.EndTime.Value - calendar.StartTime.Value;
            var rentalTime = difference.TotalHours;

            var rentalTimeString = Utils.Helpers.FormatTime((decimal)rentalTime);

            var result = await clientRepository.GetEquipmentValueByClient(calendar.ClientId, calendar.EquipamentId, rentalTimeString);

            //if (result != 0)
            //{

                var specs = await clientRepository.GetSpecsByClient(calendar.ClientId);
                if (specs.Count() > 0)
                    calendar.Value = ValuesBySpecification(calendar, specs, result);
                else
                    calendar.Value = calendar.ValueWithoutSpec = result;
            //    return;
            //}

            //throw new CalendarNoValueException("Não foi encontrado o valor para a Locação no cadastro do cliente");

        }

        private decimal ValuesBySpecification(CalendarViewModel calendar, IEnumerable<ClientSpecification> specifications, decimal valueWithoutSpec)
        {
            TimeSpan difference = calendar.EndTime.Value - calendar.StartTime.Value;
            var rentalTime = difference.TotalHours;

            calendar.ValueWithoutSpec = valueWithoutSpec;
            var specs = calendar.CalendarSpecifications.Where(x => x.Active);

            foreach (var spec in specs)
            {
                foreach(var item in specifications)
                {
                    if (item.SpecificationId == spec.SpecificationId)
                    {
                        if ( rentalTime  <= item.Hours || item.Hours == 0)
                        {
                            calendar.Additional1 = item.Value;
                            return valueWithoutSpec;
                        }   
                    }
                }
            }

            return valueWithoutSpec;
        }

        private int CalculateMinutes(DateTime startTime, DateTime endTime)
        {
            if (endTime < startTime)
                throw new ArgumentException("A data final deve ser maior ou igual à data inicial.");

            TimeSpan difference = endTime - startTime;
            return (int)difference.TotalMinutes;
        }

    }
}

