using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils.Enum;

namespace Solucao.Application.Service.Implementations
{
    public class ConsumableService : IConsumableService
	{
        private ConsumableRepository repository;
        private readonly IMapper mapper;
        private readonly HistoryRepository history;


        public ConsumableService(ConsumableRepository _repository, IMapper _mapper, HistoryRepository _history)
		{
            repository = _repository;
            mapper = _mapper;
            history = _history;
        }

        public async Task<ValidationResult> Add(ConsumableViewModel consumable, Guid loggedUserId)
        {
            consumable.Id = Guid.NewGuid();
            consumable.CreatedAt = DateTime.Now;
            var _consumable = mapper.Map<Consumable>(consumable);

            var result = await repository.Add(_consumable);

            if (result == null)
                return new ValidationResult("Houve um problema para validar o objeto");

            // adiciona a tabela de histórico de alteracao
            await history.Add(TableEnum.Consumable, result.Id, OperationEnum.Criacao, loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<IEnumerable<ConsumableViewModel>> GetAll()
        {
            return mapper.Map<IEnumerable<ConsumableViewModel>>(await repository.GetAll());
        }

        public async Task<ValidationResult> Update(ConsumableViewModel consumable, Guid loggedUserId)
        {
            consumable.Id = Guid.NewGuid();
            consumable.UpdatedAt = DateTime.Now;
            var _consumable = mapper.Map<Consumable>(consumable);

            var result = await repository.Update(_consumable);

            if (result == null)
                return new ValidationResult("Houve um problema para validar o objeto");

            // adiciona a tabela de histórico de alteracao
            await history.Add(TableEnum.Consumable, result.Id, OperationEnum.Alteracao, loggedUserId);

            return ValidationResult.Success;
        }
    }
}

