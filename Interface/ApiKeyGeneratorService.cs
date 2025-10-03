using Microsoft.EntityFrameworkCore;
using Remitplus_Accessbank_Service.Helper;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;

namespace Remitplus_Authentication.Interface
{
    public interface IApiKeysGeneratorService
    {
        Task<ApiResponse> GenerateApiKeysOperation(GenerateKeysReqDto generateKeys);
        Task<ApiResponse> GetUserApiKeyOperation();
    }

    public class ApiKeyGeneratorService(RemitplusDatabaseContext context, IEncryptionHandler encrypt, IConfiguration config) : IApiKeysGeneratorService
    {
        private readonly RemitplusDatabaseContext _context = context;
        private readonly IEncryptionHandler _encrypt = encrypt;
        private readonly IConfiguration _config = config;

        public async Task<ApiResponse> GenerateApiKeysOperation(GenerateKeysReqDto generateKeys)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == generateKeys.Email);
            if (user == null)
                return ApiResponse.Failed("Invalid user.");
            var existingKey = await _context.UserApiKeys
                .FirstOrDefaultAsync(k => k.ApplicationUserId == user.UserId);

            switch (existingKey != null)
            {
                case true:
                    existingKey.ApiKeyHash = _encrypt.AESEncryptData(ApiKeyHelper.GenerateApiKey());
                    existingKey.ExpiryDate = DateTime.UtcNow.AddDays(30).AddMinutes(-1);
                    existingKey.IsValid = true;
                    existingKey.IsDeleted = false;
                    existingKey.CreateAt = DateTime.UtcNow;
                    existingKey.CreatedById = user.UserId.ToString();

                    _context.UserApiKeys.Update(existingKey);
                    break;
                case false:
                    var apiKey = new UserApiKey
                    {
                        ApplicationUserId = user.UserId,
                        ApiKeyHash = _encrypt.AESEncryptData(ApiKeyHelper.GenerateApiKey()),
                        IsValid = true,
                        IsDeleted = false,
                        CreateAt = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddDays(30).AddMinutes(-1),
                        CreatedById = user.UserId.ToString(),
                    };
                    _context.UserApiKeys.Add(apiKey);
                    break;
            }

            await _context.SaveChangesAsync();

            return ApiResponse.Success("API key generated successfully");
        }

        public Task<ApiResponse> GetUserApiKeyOperation()
        {
            var userId = _config["sub"]?.ToString();
            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (user == null)
                return Task.FromResult(ApiResponse.Failed("Invalid user."));

            var apiKeys = _context.UserApiKeys
                .FirstOrDefault(k => k.ApplicationUserId == user.UserId && !k.IsDeleted);

            if (apiKeys == null)
                return Task.FromResult(ApiResponse.Failed("No API key found for this user."));

            return Task.FromResult(ApiResponse.Success("API key retrieved successfully", new
            {
                ApiKey = _encrypt.AESDecryptData(apiKeys.ApiKeyHash),
                Expiry = apiKeys.ExpiryDate
            }));
        }
    }
}
