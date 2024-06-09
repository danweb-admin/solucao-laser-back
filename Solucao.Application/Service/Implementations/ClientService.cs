using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Implementations
{
    public class ClientService : IClientService
    {
        private IClientRepository clientRepository;
        private IEquipamentRepository equipamentRepository;

        private readonly IMapper mapper;

        public ClientService(IClientRepository _clientRepository, IMapper _mapper, IEquipamentRepository _equipamentRepository)
        {
            clientRepository = _clientRepository;
            equipamentRepository = _equipamentRepository;
            mapper = _mapper;

        }

        public Task<ValidationResult> Add(ClientViewModel client)
        {
            client.Id = Guid.NewGuid();
            client.CreatedAt = DateTime.Now;
            var _client = mapper.Map<Client>(client);

            return clientRepository.Add(_client);
        }

        public async Task<IEnumerable<ClientViewModel>> GetAll(bool ativo, string search)
        {
            return mapper.Map<IEnumerable<ClientViewModel>>(await clientRepository.GetAll(ativo,search));
        }

        public Task<ClientViewModel> GetById(Guid Id)
        {
            return mapper.Map<Task<ClientViewModel>>(clientRepository.GetById(Id));
        }

        public Task<ValidationResult> Update(ClientViewModel client)
        {
            client.UpdatedAt = DateTime.Now;
            var _client = mapper.Map<Client>(client);

            return clientRepository.Update(_client);
        }

        public async Task<decimal> GetValueByEquipament(Guid clientId, Guid equipamentId, string startTime, string endTime)
        {
            startTime = startTime.Replace(":", "");
            endTime = endTime.Replace(":", "");

            var client = await clientRepository.GetById(clientId);
            var equipament = await equipamentRepository.GetById(equipamentId);

            var now = DateTime.Now;

            var _startTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(startTime.Substring(0, 2)), int.Parse(startTime.Substring(2)), 0);
            var _endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(endTime.Substring(0, 2)), int.Parse(endTime.Substring(2)), 0);

            TimeSpan difference = _endTime - _startTime;
            var rentalTime = difference.TotalHours;

            var split = client.EquipamentValues.Split("->");

            foreach (var line in split)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var strings = line.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var equipamento = strings[0].Trim();

                if (equipament.Name.ToUpper().Contains(equipamento))
                {

                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (i == 0)
                            continue;

                        var hoursValues = strings[i].Replace("-", "–").Split("–");

                        var hours = hoursValues[0].Trim();
                        var value = decimal.Parse(hoursValues[1].Trim().Replace(".", "").Replace(",", "."));

                        var hr = int.Parse(Regex.Replace(hours.Trim(), @"[^\d]", ""));

                        if (rentalTime <= hr)
                            return value;

                    }
                }
            }

            throw new CalendarNoValueException("Não foi encontrado o valor para a Locação no cadastro do cliente");

        }
    }
}
