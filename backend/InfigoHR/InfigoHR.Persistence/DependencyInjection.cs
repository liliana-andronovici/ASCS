using InfigoHR.Application.Interfaces;
using InfigoHR.Persistence.Services;
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

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
        services.AddScoped<ITrustedDeviceService, TrustedDeviceService>();

        return services;
    }
}
