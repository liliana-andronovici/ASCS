using Hangfire;
using Hangfire.PostgreSql;
using InfigoHR.Presentation.Jobs;

namespace InfigoHR.Presentation;

public static class HangfireServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireSetup(this IServiceCollection services, string? connectionString)
    {
        services.AddScoped<CreateEmployeeJob>();
        services.AddScoped<RoleJobs>();
        services.AddScoped<PermissionJobs>();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();

        return services;
    }
}
