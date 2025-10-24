using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Helper;
using System.Text.Json;

namespace Remitplus_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController(IEncryptionHandler encrypt) : ControllerBase
    {
        private readonly IEncryptionHandler _encrypt = encrypt;

        [HttpGet]
        public Task<IActionResult> EncryptData(dynamic data)
        {
            var response = _encrypt.AESEncryptData(JsonSerializer.Serialize(data));
            return Ok(response);
        }

        [HttpGet]
        public Task<IActionResult> DecryptData(string data)
        {
            dynamic response = _encrypt.AESDecryptData(data);
            return Ok(response);
        }
    }
}
