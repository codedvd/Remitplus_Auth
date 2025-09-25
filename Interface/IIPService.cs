
using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Context;
using Remitplus_Authentication.Model;
using Remitplus_Authentication.Model.Dtos;
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

    public class IPService(RemitPlusDbContext context) : IIPService
    {
        private readonly RemitPlusDbContext _context = context;

        public async Task<ApiResponse> BlacklistOperation(BlacklistIPReqDto reqDto)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.Equals(reqDto.Email, StringComparison.CurrentCultureIgnoreCase));
            if (user == null)
                return ApiResponse.Failed("User Not found");

            var entry = await _context.IPBlackLists
            .FirstOrDefaultAsync(x => x.IPAddress == reqDto.IpAddress);

            if (entry == null)
            {
                entry = new IPBlackList
                {
                    Id = Guid.NewGuid(),
                    IPAddress = reqDto.IpAddress
                };
                _context.IPBlackLists.Add(entry);
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
            var ipOperations = await _context.WhitelistedIpLog.Where(u => u.IpAddress == IpAddress).ToListAsync();
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
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);
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
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == reqDto.Email);
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
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == reqDto.Email);
            if (user == null)
                return ApiResponse.Failed("User not found.");

            var exists = await _context.IpWhitelists
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.UserId && x.IsActive);

            List<string> updatedIds = [];
            if(exists != null)
            {
                var ips = exists?.IpAddress.Split(',').Distinct().ToList();
                updatedIds.AddRange(ips != null ? ips.Concat(reqDto.IpAddress) : reqDto.IpAddress);
            }

            var entry = new ApplicationUserIpWhitelist
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.UserId,
                IpAddress = JsonSerializer.Serialize(updatedIds),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30).AddMinutes(-1),
                CreatedById = "SYSTEM"
            };

            _context.IpWhitelists.Add(entry);
            await _context.SaveChangesAsync();

            return ApiResponse.Success("IP whitelisted successfully", entry);
        }
    }
}
