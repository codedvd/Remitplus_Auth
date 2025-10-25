using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;
using Remitplus_Authentication.Repository;

namespace Remitplus_Authentication.Middlewares
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            Env.Load();

            // Assign environment variables to config
            config["XApiKey"] = Environment.GetEnvironmentVariable("XAPI_KEY");
            config["Encryption:Key"] = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            config["Encryption:Iv"] = Environment.GetEnvironmentVariable("ENCRYPTION_IV");
            config["BaseUrl"] = Environment.GetEnvironmentVariable("SERVICE_BASE");
            config["DatabaseConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION");

            services.AddControllers();
            services.AddScoped<IApiKeysGeneratorService, ApiKeyGeneratorService>();
            services.AddScoped<IAuthenticateUserService, AuthenticateUserService>();
            services.AddScoped<IAuthenticateRepository, AuthenticateRepository>();
            services.AddScoped<IApiKeyGeneratorRepository, ApiKeyGeneratorRepository>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IEncryptionHandler, EncryptionHandler>();
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IIPService, IPService>();
            services.AddScoped<IRestClient, RestClient>();
            services.AddHealthChecks();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Disposition");
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Remit Authentication", Version = "v1" });

                var jwtScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Enter JWT Bearer token only",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtScheme, Array.Empty<string>() }
                });

                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key must appear in header",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "XApiKey",
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddDbContext<E2epaymetsContext>(options =>
            {
                options.UseNpgsql(config["DatabaseConnection"]);
            });

             services.AddEndpointsApiExplorer();
             services.AddSwaggerGen();

            return services;
        }
    }
}
