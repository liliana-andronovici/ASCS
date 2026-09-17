using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Services;
using InfigoHR.Persistence.Repositories;
using InfigoHR.Persistence;
using EvolveDb;
using Npgsql;
using FluentValidation;
using FluentValidation.AspNetCore;
using InfigoHR.Application.Validators.Locations;
using InfigoHR.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPersistence(builder.Configuration);

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
        Locations = new[] { Path.Combine(AppContext.BaseDirectory, "db", "migrations") },
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
