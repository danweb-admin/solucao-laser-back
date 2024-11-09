﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Service.Interfaces;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private IDashboardService service;
        private CultureInfo cultureInfo = new CultureInfo("pt-BR");

        public DashboardController(IDashboardService _service)
        {
            service = _service;
        }

        [HttpGet("dashboard/locacoes-by-period")]
        public async Task<IActionResult> LocacoesByPeriod([FromQuery] DashboardRequest request)
        {
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", cultureInfo);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", cultureInfo);

            return Ok(await service.LocacoesByPeriod(startDate, endDate, request.Status));
        }
    }
}

