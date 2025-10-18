using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Remitplus_Authentication.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Remitplus_Authentication.Middlewares
{
    public class JwtValidationMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings, IConfiguration config)
    {
        private readonly RequestDelegate _next = next;
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly IConfiguration _config = config;

        //help
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            if (path.StartsWith("/api/authentication/login") || path.StartsWith("/api/authentication/create-account"))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"]
                               .FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("JWT token is missing.");
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                context.Items["UserId"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                foreach (var claim in jwtToken.Claims)
                {
                    _config[claim.Type] = claim.Value;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Invalid JWT token: {ex.Message}");
            }
        }
    }
}
