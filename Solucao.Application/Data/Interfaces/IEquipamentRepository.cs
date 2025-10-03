using System;
using Solucao.Application.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Data.Interfaces
{
	public interface IEquipamentRepository
	{
        Task<IEnumerable<Equipament>> GetAll(bool ativo);
        Task<IEnumerable<Equipament>> Get(bool ativo);
        Task<IEnumerable<Equipament>> GetListById(List<Guid> guids);
        Task<Equipament> GetById(Guid id);
        Task<ValidationResult> Add(Equipament equipament);
        Task<ValidationResult> Update(Equipament equipament);
    }
}

