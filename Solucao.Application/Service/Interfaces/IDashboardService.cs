using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solucao.Application.Contracts;

namespace Solucao.Application.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<SeriesDataViewModel>> LocacoesByPeriod(DateTime startDate, DateTime endDate, string status);
    }
}

