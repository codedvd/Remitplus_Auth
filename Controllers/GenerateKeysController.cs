using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Model.Dtos;

namespace Remitplus_Authentication.Controllers
{
    [Route("api/")]
    [ApiController]
    public class GenerateKeysController(IApiKeysGeneratorService apiKeysGenerator) : ControllerBase
    {
        private readonly IApiKeysGeneratorService _apiKeysGenerator = apiKeysGenerator;

        [HttpPost("GenerateFreshKey")]
        public async Task<IActionResult> GenerateNewApi(GenerateKeysReqDto generateKeys)
        {
            var generate = await _apiKeysGenerator.GenerateApiKeysOperation(generateKeys);
            return Ok(generate);
        }
    }
}
