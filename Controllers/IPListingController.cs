using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Model.Dtos;
using System.Threading.Tasks;

namespace Remitplus_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPListingController(IIPService ipService) : ControllerBase
    {
        private readonly IIPService _ipService = ipService;

        [HttpPost("whitelist-ip")]
        public async Task<ActionResult<ApiResponse>> WhitelistIp(WhitelistIpReqDto reqDto)
        {
            var whitelist = await _ipService.WhitelistOperation(reqDto);
            return Ok(whitelist);
        }

        [HttpPost("blacklist-ip")]
        public async Task<ActionResult<ApiResponse>> BlacklistIp(BlacklistIPReqDto reqDto)
        {
            var blacklist = await _ipService.BlacklistOperation(reqDto);
            return Ok(blacklist);
        }

        [HttpPost("get-access-logs")]
        public async Task<ActionResult<ApiResponse>> GetAccessLogs(int? pageSize, int? pageNumber, string ipAddress)
        {
            var getlogs = await _ipService.GetIPLogsOperation(pageSize, pageNumber, ipAddress);
            return Ok(getlogs);
        }

        [HttpPost("remove-whitelisted-ip")]
        public async Task<ActionResult<ApiResponse>> RemoveWhitelistedIp(WhitelistIpReqDto reqDto)
        {
            var removewhitelist = await _ipService.RemoveWhitelistOperation(reqDto);
            return Ok();
        }

        [HttpGet("get-whitelisted-ips")]
        public async Task<ActionResult<ApiResponse>> GetWhitelistedIp(string email)
        {
            var getwhitelistedips = await _ipService.GetWhitelistedIPsOperation(email);
            return Ok(getwhitelistedips);
        }


    }
}
