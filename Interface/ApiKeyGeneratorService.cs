using Microsoft.EntityFrameworkCore;
using Remitplus_Accessbank_Service.Helper;
using Remitplus_Authentication.Context;
using Remitplus_Authentication.Model;
using Remitplus_Authentication.Model.Dtos;
using System.Security.Cryptography;
using System.Text;

namespace Remitplus_Authentication.Interface
{
    public interface IApiKeysGeneratorService
    {
        Task<ApiResponse> GenerateApiKeysOperation(GenerateKeysReqDto generateKeys);
    }

    public class ApiKeyGeneratorService(RemitPlusDbContext context, IEncryptionHandler encrypt) : IApiKeysGeneratorService
    {
        private readonly RemitPlusDbContext _context = context;
        private readonly IEncryptionHandler _encrypt = encrypt;

        public async Task<ApiResponse> GenerateApiKeysOperation(GenerateKeysReqDto generateKeys)
        {
            // validate input
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == generateKeys.Email);
            if (user == null)
                return ApiResponse.Failed("Invalid user.");

            var rawKey = ApiKeyHelper.GenerateApiKey();

            var apiKey = new ApplicationUserApiKeys
            {
                ApplicationUserId = user.UserId.ToString(),
                ApiKeyHash = _encrypt.AESEncryptData(rawKey),
                IsValid = true,
                IsDeleted = false,
                CreateAt = DateTime.UtcNow,
                CreatedById = user.UserId.ToString(),
            };

            _context.ApplicationUserApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            return ApiResponse.Success("API key generated successfully", new
            {
                ApiKey = rawKey,
                UserId = user.UserId
            });
        }
    }
}
