using FluentValidation;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InfigoHR.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IOrgChartService, OrgChartService>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}