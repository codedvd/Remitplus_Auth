namespace Remitplus_Authentication.Models.Dtos;

public class GenerateKeysReqDto
{
    public string Email { get; set; } = string.Empty;
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

public class ActivateApiReq
{
    public bool IsActive { get; set; }
}