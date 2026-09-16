using EvolveDb;
using FluentValidation;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.Validators.Departments;
using InfigoHR.Persistence;
using InfigoHR.Presentation.Middleware;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddScoped<IValidator<CreateDepartmentModel>, CreateDepartmentModelValidator>();
builder.Services.AddScoped<IValidator<UpdateDepartmentModel>, UpdateDepartmentModelValidator>();

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

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
