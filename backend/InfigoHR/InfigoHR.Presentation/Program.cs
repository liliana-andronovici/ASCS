using EmailService;
using EvolveDb;
using InfigoHR.Application;
using InfigoHR.Application.Services;
using InfigoHR.Persistence;
using InfigoHR.Presentation.Middleware;
using Npgsql;
using EmailServiceComponent = EmailService.EmailService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

var smtpSettings = builder.Configuration
    .GetSection("Smtp")
    .Get<SmtpSettings>()
    ?? throw new InvalidOperationException("SMTP configuration is missing.");

var registrationBaseUrl = builder.Configuration["Links:Registration"]
    ?? throw new InvalidOperationException("Registration URL configuration is missing.");

builder.Services.AddSingleton(smtpSettings);
builder.Services.AddSingleton<IEmailServiceStore, InMemoryEmailServiceStore>();
builder.Services.AddScoped<EmailServiceComponent>();
builder.Services.AddScoped<MailKitEmailSender>();
builder.Services.AddScoped<InvitationLink>(sp => new InvitationLink(
    sp.GetRequiredService<EmailServiceComponent>(),
    sp.GetRequiredService<MailKitEmailSender>(),
    registrationBaseUrl));

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
