using FluentValidation;
using InfigoHR.Application;
using InfigoHR.EmailService.Configuration;
using InfigoHR.EmailService.Interfaces;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Services;
using InfigoHR.Application.Validators.Locations;
using InfigoHR.Authentication;
using InfigoHR.Persistence;
using InfigoHR.Presentation.Configurations;
using InfigoHR.Persistence.Repositories;
using InfigoHR.Presentation.Middleware;
using InfigoHR.EmailService.Services;
using InfigoHR.Presentation;
using Hangfire;
using Hangfire.Dashboard;
using System.Net;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationModelValidator>();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddRateLimiter(_ => { });
builder.Services.ConfigureOptions<PasswordResetRateLimiterSetup>();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHangfireSetup(connectionString);

var app = builder.Build();

DatabaseSetup.MigrateDatabase(connectionString!);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new DashboardBasicAuth(builder.Configuration)]
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();

app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

sealed class DashboardBasicAuth(IConfiguration configuration) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        var remote = http.Connection.RemoteIpAddress;
        if (!http.Request.IsHttps && (remote is null ||
            !IPAddress.IsLoopback(remote) &&
            (!remote.IsIPv4MappedToIPv6 || !IPAddress.IsLoopback(remote.MapToIPv4()))))
            return false;

        var username = configuration["HangfireDashboard:Username"];
        var password = configuration["HangfireDashboard:Password"];
        var expected = Encoding.UTF8.GetBytes("Basic " + Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{username}:{password}")));
        var actual = Encoding.UTF8.GetBytes(http.Request.Headers["Authorization"].ToString());
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(password) &&
            CryptographicOperations.FixedTimeEquals(actual, expected))
            return true;

        http.Response.StatusCode = StatusCodes.Status401Unauthorized;
        http.Response.Headers["WWW-Authenticate"] = "Basic realm=\"InfigoHR Hangfire\", charset=\"UTF-8\"";
        return false;
    }
}
