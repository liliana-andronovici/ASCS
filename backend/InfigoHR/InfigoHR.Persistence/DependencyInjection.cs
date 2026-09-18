using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Services;
using InfigoHR.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfigoHR.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<AuthRepository>();

        return services;
    }
}
