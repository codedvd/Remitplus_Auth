using Remitplus_Authentication.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Serve static files (your React build)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.MapWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    apiApp =>
    {
        app.UseMiddleware<JwtValidationMiddleware>();
        app.UseMiddleware<ApiKeyMiddleware>();

        apiApp.UseRouting();
        apiApp.UseAuthorization();
        apiApp.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
);

app.UseAuthentication();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
