using FluentValidation;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Jwt;
using InfigoHR.Authentication.Models;
using InfigoHR.Authentication.Services;
using InfigoHR.Authentication.Validators;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfigoHR.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(nameof(JwtOptions)));

        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        services.AddScoped<
            IPasswordHasher<AuthEntity>,
            PasswordHasher<AuthEntity>>();

        services.AddScoped<
            IValidator<SignInRequest>,
            SignInRequestValidator>();

        services.AddScoped<
            IValidator<SignUpRequest>,
            SignUpRequestValidator>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthorization(options =>
        {
            foreach (var permission in Enum.GetValues<Permission>())
            {
                var permissionName = permission.ToString();

                options.AddPolicy(permissionName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("permissions", permissionName);
                });
            }
        });

        return services;
    }
}