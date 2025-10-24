using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Models.Dtos;

namespace Remitplus_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticateUserService authenticate) : ControllerBase
    {
        private readonly IAuthenticateUserService _authenticate = authenticate;

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(OnboardUserReqDto userReqDto)
        {
            ApiResponse create = await _authenticate.CreateANewUser(userReqDto);
            return Ok(create);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginReqDto loginReq)
        {
            ApiResponse login = await _authenticate.LoginRegisteredUserlo(loginReq);
            return Ok(login);
        }

        [HttpPost("get-all-users")]
        public async Task<IActionResult> GetInAppUser(SortUser SortUser)
        {
            return Ok(await _authenticate.GetAllUser(SortUser));
        }

        [HttpGet("get-user-analytics")]
        public async Task<IActionResult> GetUserAnalytics()
        {
            return Ok(await _authenticate.GetUserAnalytics());
        }

        [HttpGet("get-all-roles")]
        public async Task<IActionResult> GetAllUserROles()
        {
            return Ok(await _authenticate.GetAllUserROles());
        }

        [HttpPost]
        [Route("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserReqDto reqDto)
        {
            return Ok(await _authenticate.UpdateUserOperation(reqDto));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordReqDto resetPassword)
        {
            ApiResponse reset = await _authenticate.ResetPasswordOperation(resetPassword);
            return Ok(reset);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPassReqDto forgetPassReq)
        {
            ApiResponse forget = await _authenticate.ForgetPasswordOperation(forgetPassReq);
            return Ok(forget);
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var delResponse = await _authenticate.DeleteUserAsync(userId);
            return Ok(delResponse);
        }

    }
}
