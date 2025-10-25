using Microsoft.EntityFrameworkCore;
using Remitplus_Accessbank_Service.Helper;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;

namespace Remitplus_Authentication.Interface
{
    public interface IApiKeysGeneratorService
    {
        Task<ApiResponse> ActivateDeactvateKey(ActivateApiReq apiReq);
        Task<ApiResponse> DeleteApiKey(string keyId);
        Task<ApiResponse> GenerateApiKeysOperation(GenerateKeysReqDto generateKeys);
        Task<ApiResponse> GenerateNewEncryptionKeys();
        Task<ApiResponse> GetUserApiKeyOperation();
    }

    public class ApiKeyGeneratorService(E2epaymetsContext context, IEncryptionHandler encrypt, IConfiguration config) : IApiKeysGeneratorService
    {
        private readonly E2epaymetsContext _context = context;
        private readonly IEncryptionHandler _encrypt = encrypt;
        private readonly IConfiguration _config = config;

        public async Task<ApiResponse> ActivateDeactvateKey(ActivateApiReq apiReq)
        {
            var userId = _config["sub"]?.ToString();
            if(userId == null)
            {
                return ApiResponse.Failed("User is not authorized");
            }
            var apiKey = await (from a in _context.UserApiKeys
                                where a.ApplicationUserId == Guid.Parse(userId)
                                select a).FirstOrDefaultAsync();
            if (apiKey == null)
                return ApiResponse.Failed("Api Key does not exist!");

            apiKey.IsActive = apiReq.IsActive;
            _context.Update(apiKey);
            await _context.SaveChangesAsync();
            string message = apiReq.IsActive ? "Activated" : "Deactivated";
            return ApiResponse.Success($"Api has been {message} successfully.");
        }

        public async Task<ApiResponse> DeleteApiKey(string keyId)
        {
            var userId = _config["sub"]?.ToString();
            if(userId == null)
                return ApiResponse.Failed($"Unable to delete {keyId}");
            var apikey = await _context.UserApiKeys.FirstOrDefaultAsync(k => k.Id == int.Parse(keyId)
                        && k.ApplicationUserId == Guid.Parse(userId));

            if (apikey == null)
                return ApiResponse.Failed("Apikey is not found.");

            apikey.IsDeleted = true;
            apikey.IsActive = false;
            _context.Update(apikey);
            await _context.SaveChangesAsync();

            return ApiResponse.Success($"Api key with id: {keyId} has been deleted");
        }

        public async Task<ApiResponse> GenerateApiKeysOperation(GenerateKeysReqDto generateKeys)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == generateKeys.Email);
            if (user == null)
                return ApiResponse.Failed("Invalid user.");
            var existingKey = await _context.UserApiKeys
                .FirstOrDefaultAsync(k => k.ApplicationUserId == user.UserId);
            var newApiKey = ApiKeyHelper.GenerateApiKey();

            switch (existingKey != null)
            {
                case true:
                    existingKey.ApiKeyHash = _encrypt.AESEncryptData(newApiKey);
                    existingKey.ExpiryDate = DateTime.UtcNow.AddDays(30).AddMinutes(-1);
                    existingKey.IsActive = true;
                    existingKey.IsDeleted = false;
                    existingKey.CreateAt = DateTime.UtcNow;
                    existingKey.CreatedById = user.UserId.ToString();

                    _context.UserApiKeys.Update(existingKey);
                    break;
                case false:
                    var apiKey = new UserApiKey
                    {
                        ApplicationUserId = user.UserId,
                        ApiKeyHash = _encrypt.AESEncryptData(newApiKey),
                        IsActive = true,
                        IsDeleted = false,
                        CreateAt = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddDays(30).AddMinutes(-1),
                        CreatedById = user.UserId.ToString(),
                    };

                    _context.UserApiKeys.Add(apiKey);
                    break;
            }

            await _context.SaveChangesAsync();

            return ApiResponse.Success("API key generated successfully", newApiKey);
        }

        public async Task<ApiResponse> GenerateNewEncryptionKeys()
        {
            var userId = _config["sub"]?.ToString();
            if (userId == null)
                return ApiResponse.Failed("Unable to generate keys, contact admin!");

            var userKeys = await (from ks in _context.UserApiKeys
                                  where ks.ApplicationUserId == Guid.Parse(userId)
                                  select ks).FirstOrDefaultAsync();
            if (userKeys == null)
                return ApiResponse.Failed("No field key for user is found.");

            var (Key, IV) = CryptoGenerator.GenerateKeyAndIV();
            _context.UserApiKeys.Update(userKeys);
            await _context.SaveChangesAsync();

            return ApiResponse.Success("Encryption key generated successfully.");
        }

        public Task<ApiResponse> GetUserApiKeyOperation()
        {
            var userId = _config["sub"]?.ToString();
            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (user == null)
                return Task.FromResult(ApiResponse.Failed("Invalid user."));

            var apiKeys = _context.UserApiKeys
                .Where(k => k.ApplicationUserId == user.UserId && !k.IsDeleted).ToList();

            if (apiKeys == null)
                return Task.FromResult(ApiResponse.Failed("No API key found for this user."));

            return Task.FromResult(ApiResponse.Success("API key retrieved successfully", apiKeys.Select(a => new GetKeyResponse
            {
                ApiKey = _encrypt.AESDecryptData(a.ApiKeyHash),
                Id = a.Id,
                IsActive = a.IsActive,
                LastUsed = a.LastUsed.ToString() ?? "",
                CreatedAt = a.CreateAt.ToString(),
                Expiry = a.ExpiryDate.ToString(),
            })));
        }
    }
}
