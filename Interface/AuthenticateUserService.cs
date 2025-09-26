using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Context;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Model;
using Remitplus_Authentication.Model.Dtos;

namespace Remitplus_Authentication.Interface
{
    public interface IAuthenticateUserService
    {
        Task<ApiResponse> CreateANewUser(OnboardUserReqDto userReqDto);
        Task<ApiResponse> ForgetPasswordOperation(ForgetPassReqDto forgetPassReq);
        Task<ApiResponse> LoginRegisteredUserlo(LoginReqDto loginReq);
        Task<ApiResponse> ResetPasswordOperation(ResetPasswordReqDto resetPassword);
    }

    public class AuthenticateUserService(RemitPlusDbContext context, IEncryptionHandler encrypt, IJwtService jwtService) : IAuthenticateUserService
    {
        private readonly RemitPlusDbContext _context = context;
        private readonly IEncryptionHandler _encrypt = encrypt;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<ApiResponse> CreateANewUser(OnboardUserReqDto userReqDto)
        {
            // check if email already exists
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email.Equals(userReqDto.Email));
            if (user != null)
                return ApiResponse.Failed("Email already registered.");

            var newUser = new ApplicationUser
            {
                FullName = userReqDto.FullName,
                Email = userReqDto.Email,
                PasswordHash = _encrypt.AESEncryptData(userReqDto.Password),
                PhoneNumber = userReqDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.MinValue
            };
            
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return ApiResponse.Success("User created successfully", newUser);
        }

        public async Task<ApiResponse> ForgetPasswordOperation(ForgetPassReqDto forgetPassReq)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgetPassReq.Email);
            if (user == null)
                return ApiResponse.Failed("No account found with this email.");

            // TODO: Send reset token via email in real implementation
            return ApiResponse.Success("Password reset link sent to your email.");
        }

        public async Task<ApiResponse> LoginRegisteredUserlo(LoginReqDto loginReq)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginReq.Email);
            if (user == null)
                return ApiResponse.Failed("Invalid email or password.");

            if (_encrypt.AESEncryptData(loginReq.Password) != user.PasswordHash)
                return ApiResponse.Failed("Invalid email or password.");

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user.UserId, user.FullName);

            return ApiResponse.Success("Login successful", new
            {
                Token = token,
                Name = user.FullName
            });
        }

        public async Task<ApiResponse> ResetPasswordOperation(ResetPasswordReqDto resetPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetPassword.Email);
            if (user == null)
                return ApiResponse.Failed("No account found with this email.");

            if (_encrypt.AESEncryptData(resetPassword.OldPassword) != user.PasswordHash)
                return ApiResponse.Failed("Password Supplied is Incorrect");

            user.PasswordHash = _encrypt.AESEncryptData(resetPassword.Password);
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return ApiResponse.Success("Password reset successful.");
        }
    }
}
