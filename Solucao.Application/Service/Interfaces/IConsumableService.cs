using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Service.Interfaces
{
	public interface IConsumableService
	{
        Task<IEnumerable<ConsumableViewModel>> GetAll();

        Task<ValidationResult> Add(ConsumableViewModel consumable, Guid loggedUserId);

        Task<ValidationResult> Update(ConsumableViewModel consumable, Guid loggedUserId);
    }
}

