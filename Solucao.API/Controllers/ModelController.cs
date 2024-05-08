using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Solucao.API.Controllers
{
    [Route("api/v1/model")]
    [ApiController]
    [Authorize]
    public class ModelController : ControllerBase
    {
        private readonly IModelService service;

        public ModelController(IModelService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(bool active)
        {
            var modelos = await service.GetAll();
            return Ok(modelos);
        }

        [HttpPost()]
        public async Task<IActionResult> PostAsync([FromBody] ModelViewModel model)
        {
            try
            {

                var result = await service.Add(model);

                if (result != null)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NoContent();
            }

        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult UploadAsync()
        {
            try
            {
                var file = Request.Form.Files[0];
                var pathToSave = Environment.GetEnvironmentVariable("ModelDocsPath");
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);

                    if (Directory.Exists(pathToSave))
                        Directory.CreateDirectory(pathToSave);

                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);

                    using (var stream = new FileStream(fullPath, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite))
                    {
                        file.CopyToAsync(stream);
                    }
                    return Ok(new { fullPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] ModelViewModel model)
        {
            var result = await service.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }
    }
}

