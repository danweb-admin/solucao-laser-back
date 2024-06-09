using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ConsumablesController : ControllerBase
    {
        private IConsumableService service;
        private readonly IUserService userService;
        public ConsumablesController(IConsumableService _service, IUserService _userService)
        {
            service = _service;
            userService = _userService;
        }
        [HttpGet("consumables")]
        public async Task<IEnumerable<ConsumableViewModel>> GetAllAsync()
        {
            var res = await service.GetAll();
            return res;
        }

        [HttpPost("consumables")]
        public async Task<IActionResult> PostAsync([FromBody] ConsumableViewModel model)
        {
            var user = await userService.GetByName(User.Identity.Name);

            var result = await service.Add(model, user.Id);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }


        [HttpPut("consumables/{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] ConsumableViewModel model)
        {
            var user = await userService.GetByName(User.Identity.Name);

            var result = await service.Update(model,user.Id);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }
    }
}
