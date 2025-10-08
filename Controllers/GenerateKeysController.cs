using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Models.Dtos;

namespace Remitplus_Authentication.Controllers
{
    [Route("api/")]
    [ApiController]
    public class GenerateKeysController(IApiKeysGeneratorService apiKeysGenerator) : ControllerBase
    {
        private readonly IApiKeysGeneratorService _apiKeysGenerator = apiKeysGenerator;

        [HttpPost("generate-fresh-key")]
        public async Task<IActionResult> GenerateNewApi(GenerateKeysReqDto generateKeys)
        {
            var generate = await _apiKeysGenerator.GenerateApiKeysOperation(generateKeys);
            return Ok(generate);
        }

        [HttpGet("get-api-key")]
        public async Task<IActionResult> GetUserApiKey()
        {
            var getkeys = await _apiKeysGenerator.GetUserApiKeyOperation();
            return Ok(getkeys);
        }

        [HttpPost("action-api-key")]
        public async Task<ActionResult<ApiResponse>> ActiveDeactivateAPIKey(ActivateApiReq apiReq)
        {
            return(await _apiKeysGenerator.ActivateDeactvateKey(apiReq));
        }

        [HttpDelete("delete-api-key")]
        public async Task<ActionResult<ApiResponse>> DeleteApiKey(string keyId)
        {
            return (await _apiKeysGenerator.DeleteApiKey(keyId));
        }
    }
}
