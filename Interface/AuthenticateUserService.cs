using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;

namespace Remitplus_Authentication.Interface
{
    public interface IAuthenticateUserService
    {
        Task<ApiResponse> CreateANewUser(OnboardUserReqDto userReqDto);
        Task<ApiResponse> ForgetPasswordOperation(ForgetPassReqDto forgetPassReq);
        Task<ApiResponse> GetAllUser(SortUser sort);
        Task<ApiResponse> GetAllUserROles();
        Task<ApiResponse> GetUserAnalytics();
        Task<ApiResponse> LoginRegisteredUserlo(LoginReqDto loginReq);
        Task<ApiResponse> ResetPasswordOperation(ResetPasswordReqDto resetPassword);
        Task<ApiResponse> UpdateUserOperation(UpdateUserReqDto reqDto);
    }

    public class AuthenticateUserService(RemitplusDatabaseContext context, IEncryptionHandler encrypt, IJwtService jwtService) : IAuthenticateUserService
    {
        private readonly RemitplusDatabaseContext _context = context;
        private readonly IEncryptionHandler _encrypt = encrypt;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<ApiResponse> CreateANewUser(OnboardUserReqDto userReqDto)
        {
            // check if email already exists
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email.Equals(userReqDto.Email));
            if (user != null)
                return ApiResponse.Failed("Email already registered.");
            var roleExists = await _context.UserRoles.AsNoTracking().ToListAsync();

            var newUser = new User
            {
                FullName = userReqDto.FullName,
                Email = userReqDto.Email,
                PasswordHash = _encrypt.AESEncryptData(userReqDto.Password),
                PhoneNumber = userReqDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.MinValue,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Status = Status.Pending.ToString(),
                RoleId = roleExists.FirstOrDefault(r => r.RoleName == userReqDto.Role)?.RoleId
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

        public async Task<ApiResponse> GetAllUser(SortUser sort)
        {
            var query = await(from u in _context.Users
                        join r in _context.UserRoles on u.RoleId equals r.RoleId
                        select new
                        {
                            UserId = u.UserId,
                            FullName = u.FullName,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            IsActive = u.IsActive,
                            CreatedAt = u.CreatedAt,
                            LastLoginAt = u.LastLoginAt,
                            Status = u.Status,
                            Role = r.RoleName
                        }).ToListAsync();

            if (!string.IsNullOrWhiteSpace(sort.Search))
            {
                query = [.. query.Where(u => u.FullName.Contains(sort.Search, StringComparison.CurrentCultureIgnoreCase) || u.Email.Contains(sort.Search, StringComparison.CurrentCultureIgnoreCase))];
            }

            if (sort.Status != UserStatus.All)
            {
                bool isActive = sort.Status == UserStatus.Active;
                query = [.. query.Where(u => u.IsActive == isActive)];
            }

            if (sort.UserType != UserType.All)
            {
                string roleName = sort.UserType == UserType.Admin ? "Admin" : "User";
                query = [.. query.Where(u => u.Role == roleName)];
            }

            return ApiResponse.Success("Users retrieved successfully", query);
        }

        public async Task<ApiResponse> GetAllUserROles()
        {
            var roles = await _context.UserRoles.AsNoTracking().ToListAsync();
            if(roles.Count == 0)
                return ApiResponse.Failed("No roles found");

            return ApiResponse.Success("Roles retrieved successfully", roles.Select(r => new
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                RoleDescription = r.RoleDescription
            }));
        }

        public async Task<ApiResponse> GetUserAnalytics()
        {
            var allUsers = await(from u  in _context.Users
                                 select u).ToListAsync();
            var analytics = new
            {
                TotalUsers = allUsers.Count,
                ActiveUsers = allUsers.Where(u => u.Status == Status.Active.ToString()).Count(),
                InActiveUser = allUsers.Where(u => u.Status == Status.InActive.ToString()).Count(),
                PendingUsers = allUsers.Where(u => u.Status == Status.Pending.ToString()).Count()
            };
            return ApiResponse.Success("Analytics returned successful", analytics);
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
            string? role = await (from r in _context.UserRoles
                           where r.RoleId == user.RoleId
                           select r.RoleName).FirstOrDefaultAsync();

            return ApiResponse.Success("Login successful", new
            {
                Token = token,
                Fullname = user.FullName,
                Email = user.Email,
                UserId = user.UserId,
                LastLogin = user.LastLoginAt,
                Role = role
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

        public async Task<ApiResponse> UpdateUserOperation(UpdateUserReqDto reqDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == Guid.Parse(reqDto.UserId));
            if (user == null)
                return ApiResponse.Failed("User Not found");

            user.Status = reqDto.Status;
            user.RoleId = Guid.Parse(reqDto.RoleId);
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            return ApiResponse.Success("User updated successfully", user);
        }


    }
}
