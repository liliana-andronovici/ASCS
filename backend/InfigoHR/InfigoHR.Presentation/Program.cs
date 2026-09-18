using EvolveDb;
using FluentValidation;
using FluentValidation.AspNetCore;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.Models.Locations;
using InfigoHR.Application.Services;
using InfigoHR.Application.Validators.Departments;
using InfigoHR.Application.Validators.Locations;
using InfigoHR.Persistence;
using InfigoHR.Persistence.Repositories;
using InfigoHR.Presentation.Middleware;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddScoped<IValidator<CreateDepartmentModel>, CreateDepartmentModelValidator>();
builder.Services.AddScoped<IValidator<UpdateDepartmentModel>, UpdateDepartmentModelValidator>();

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