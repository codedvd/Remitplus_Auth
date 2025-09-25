using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Model.Dtos;

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
            return Ok(loginReq);
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

    }
}
