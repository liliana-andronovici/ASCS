using EvolveDb;
using FluentValidation;
using FluentValidation.AspNetCore;
using InfigoHR.Application;
using InfigoHR.EmailService.Configuration;
using InfigoHR.EmailService.Interfaces;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Services;
using InfigoHR.Application.Validators.Locations;
using InfigoHR.Persistence;
using InfigoHR.Persistence.Repositories;
using InfigoHR.Presentation.Middleware;
using Npgsql;
using EmailServiceImplementation = InfigoHR.EmailService.Services.EmailService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailServiceImplementation>();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationModelValidator>();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

try
{
    using var connection = new NpgsqlConnection(connectionString);
    var evolve = new Evolve(connection, msg => Console.WriteLine(msg))
    {
        Locations = new[] { Path.Combine(AppContext.BaseDirectory, "Migrations") },
        IsEraseDisabled = true
    };
    evolve.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine("Database migration failed: " + ex.Message);
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
