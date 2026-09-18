using EvolveDb;
using FluentValidation;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.Validators.Departments;
using InfigoHR.Persistence;
using InfigoHR.Presentation.Middleware;
using Npgsql;
using FluentValidation;
using InfigoHR.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));builder.Services.AddOpenApi();
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
app.UseMiddleware<InfigoHR.Presentation.Middleware.ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
