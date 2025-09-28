using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;
using System.Text.Json;

namespace Remitplus_Authentication.Interface
{
    public interface IIPService
    {
        Task<ApiResponse> BlacklistOperation(BlacklistIPReqDto reqDto);
        Task<ApiResponse> GetIPLogsOperation(int? pageSize, int? pageNumber, string IpAddress);
        Task<ApiResponse> GetWhitelistedIPsOperation(string email);
        Task<ApiResponse> RemoveWhitelistOperation(WhitelistIpReqDto reqDto);
        Task<ApiResponse> WhitelistOperation(WhitelistIpReqDto reqDto);
    }

    public class IPService(RemitplusDatabaseContext context) : IIPService
    {
        private readonly RemitplusDatabaseContext _context = context;

        public async Task<ApiResponse> BlacklistOperation(BlacklistIPReqDto reqDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(reqDto.Email.ToLower()));
            if (user == null)
                return ApiResponse.Failed("User Not found");

            var entry = await _context.IpblackLists
            .FirstOrDefaultAsync(x => x.Ipaddress == reqDto.IpAddress);

            if (entry == null)
            {
                entry = new IpblackList
                {
                    Id = Guid.NewGuid(),
                    Ipaddress = reqDto.IpAddress
                };
                _context.IpblackLists.Add(entry);
            }
            else
            {
                entry.IsActive = false;
                entry.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return ApiResponse.Success("IP blacklisted successfully.");
        }

        public async Task<ApiResponse> GetIPLogsOperation(int? pageSize, int? pageNumber, string IpAddress)
        {
            var ipOperations = await _context.WhitelistedIpLogs.Where(u => u.IpAddress == IpAddress).ToListAsync();
            if (!ipOperations.Any())
                return ApiResponse.Failed("No whitelist entries found.");

            var whitelistedIps =ipOperations
                .Select(w => new
                {
                    IPAddress = w.IpAddress,
                    ApiRoute = w.ApiRoute,
                    HttpMethod = w.HttpMethod,
                    HttpStatus = w.StatusCode,
                    RequestPayload = w.RequestPayload,
                    ResponsePayload = w.ResponsePayload,
                });

            if(pageSize == null || pageSize <= 0 || pageNumber == 0 || pageNumber == null)
            {
                whitelistedIps = whitelistedIps
                .Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10);
            }

            return ApiResponse.Success("Whitelisted IPs retrieved successfully.", whitelistedIps);
        }

        public async Task<ApiResponse> GetWhitelistedIPsOperation(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return ApiResponse.Failed("User not found.");

            var whitelistedIps = await _context.IpWhitelists
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.UserId);
            if (whitelistedIps == null)
                return ApiResponse.Failed("No whitelist entry found for this user.");

            var whitelistedIpList = string.IsNullOrWhiteSpace(whitelistedIps.IpAddress)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(whitelistedIps.IpAddress) ?? [];

            return ApiResponse.Success("Whitelisted IPs retrieved successfully.", whitelistedIpList);
        }

        public async Task<ApiResponse> RemoveWhitelistOperation(WhitelistIpReqDto reqDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == reqDto.Email);
            if (user == null)
                return ApiResponse.Failed("User not found.");

            var entry = await _context.IpWhitelists
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.UserId);

            if (entry == null)
                return ApiResponse.Failed("No whitelist entry found for this user.");

            var currentIps = string.IsNullOrWhiteSpace(entry.IpAddress)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(entry.IpAddress) ?? new List<string>();

            currentIps.RemoveAll(ip => reqDto.IpAddress.Contains(ip));

            entry.IpAddress = JsonSerializer.Serialize(currentIps);
            entry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse.Success("IPs removed from whitelist.", currentIps);
        }

        public async Task<ApiResponse> WhitelistOperation(WhitelistIpReqDto reqDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == reqDto.Email);
            if (user == null)
                return ApiResponse.Failed("User not found.");

            var exists = await _context.IpWhitelists
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.UserId && x.IsActive);

            List<string> updatedIds = [];
            switch(exists is not null)
            {
                case true:
                    var deserial = JsonSerializer.Deserialize<List<string>>(exists.IpAddress);
                    updatedIds.AddRange(deserial?.Count > 0 ? deserial.Concat(reqDto.IpAddress) : reqDto.IpAddress);

                    exists.UpdatedAt = DateTime.UtcNow;
                    exists.IpAddress = JsonSerializer.Serialize(updatedIds.Distinct());
                    _context.IpWhitelists.Update(exists);
                    break;
                case false:
                    var entry = new IpWhitelist
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = user.UserId,
                        IpAddress = JsonSerializer.Serialize(reqDto.IpAddress),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = "SYSTEM"
                    };
                    _context.IpWhitelists.Add(entry);
                    break;
            }
            await _context.SaveChangesAsync();

            return ApiResponse.Success("IP whitelisted successfully");
        }
    }
}
