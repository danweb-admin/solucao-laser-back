using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class EquipamentController : ControllerBase
    {
        private readonly IEquipamentService service;
        private readonly IUserService userService;


        public EquipamentController(IEquipamentService _service, IUserService _userService)
        {
            service = _service;
            userService = _userService;
        }

        [HttpGet("equipaments")]
        public async Task<IActionResult> GetAsync([FromQuery] EquipamentRequest request)
        {
            var result = await service.GetAll(request.Ativo);

            return Ok(result);
        }

        [HttpGet("equipaments/get-all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] EquipamentRequest request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                return NotFound("Token não fornecido. Entre em contato com o suporte.");

            var user = await userService.GetByToken(token.Replace("Bearer ", ""));

            if (user == null)
                return NotFound("Você não tem permissão para visualizar os dados dessa página. Entre em contato com o suporte.");

            var hoje = DateTime.Now;

            if (hoje.Date > user.Token_Expire.Value.Date)
                return NotFound("Token expirado. Entre em contato com o suporte.");

            var result = await service.GetAll(request.Ativo);

            return Ok(result);
        }

        [HttpPost("equipaments")]
        public async Task<IActionResult> PostAsync([FromBody] EquipamentViewModel model)
        {

            var result = await service.Add(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }


        [HttpPut("equipaments/{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] EquipamentViewModel model)
        {
            var result = await service.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }
    }
}
