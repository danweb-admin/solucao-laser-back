using Solucao.Application.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientViewModel>> GetAll(bool ativo, string search);

        Task<decimal> GetValueByEquipament(Guid clientId, Guid equipamentId, string startTime, string endTime);

        Task<ClientViewModel> GetById(Guid Id);

        Task<ValidationResult> Add(ClientViewModel client);

        Task<ValidationResult> Update(ClientViewModel client);
    }
}
