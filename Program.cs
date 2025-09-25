using Microsoft.EntityFrameworkCore;
using Remitplus_Accessbank_Service.Helper;
using Remitplus_Authentication.Context;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Model.Dtos;
using Remitplus_Authentication.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IApiKeysGeneratorService, ApiKeyGeneratorService>();
builder.Services.AddScoped<IAuthenticateUserService, AuthenticateUserService>();
builder.Services.AddScoped<IAuthenticateRepository, AuthenticateRepository>();
builder.Services.AddScoped<IApiKeyGeneratorRepository, ApiKeyGeneratorRepository>();
builder.Services.AddScoped<IEncryptionHandler, EncryptionHandler>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddDbContext<RemitPlusDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConntion"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RemitPlusDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Applying database migrations...");
        db.Database.Migrate();
        logger.LogInformation("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply database migrations.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
