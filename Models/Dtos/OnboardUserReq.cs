using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Remitplus_Authentication.Models.Dtos;

public class OnboardUserReqDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class LoginReqDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ResetPasswordReqDto
{
    public string Email { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ForgetPassReqDto
{
    public string Email { get; set; } = string.Empty;
}

public class WhitelistIpReqDto
{
    public string Email { get; set; } = string.Empty;
    public List<string> IpAddress { get; set; } = [];
}

public class BlacklistIPReqDto
{
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}