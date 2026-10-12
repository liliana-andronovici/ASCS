using EvolveDb;
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
using InfigoHR.Persistence.Repositories;
using InfigoHR.Presentation.Middleware;
using InfigoHR.EmailService.Services;
using InfigoHR.Presentation;
using Hangfire;

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHangfireSetup(connectionString);

var app = builder.Build();

DatabaseSetup.MigrateDatabase(connectionString!);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHangfireDashboard("/hangfire");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
