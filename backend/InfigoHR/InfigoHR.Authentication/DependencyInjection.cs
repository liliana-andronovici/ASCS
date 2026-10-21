using FluentValidation;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Jwt;
using InfigoHR.Authentication.Models;
using InfigoHR.Authentication.Options;
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

        services.AddOptions<PasswordResetOptions>()
            .Bind(configuration.GetSection(nameof(PasswordResetOptions)))
            .Validate(
                HasValidCodeHashKey,
                "PasswordResetOptions:CodeHashKey must contain at least 32 random bytes encoded as Base64.")
            .ValidateOnStart();

        services.AddSingleton<
            IPasswordResetTokenProvider,
            PasswordResetTokenProvider>();

        services.AddScoped<
            IPasswordResetService,
            PasswordResetService>();

        services.AddScoped<
            IValidator<RequestPasswordResetRequest>,
            RequestPasswordResetRequestValidator>();

        services.AddScoped<
            IValidator<VerifyPasswordResetRequest>,
            VerifyPasswordResetRequestValidator>();

        services.AddScoped<
            IValidator<CompletePasswordResetRequest>,
            CompletePasswordResetRequestValidator>();

        services.AddScoped<
            IPasswordResetEmailService,
            PasswordResetEmailService>();

        services.AddOptions<TwoFactorOptions>()
            .Bind(configuration.GetSection(nameof(TwoFactorOptions)));
        services.AddDataProtection();

        services.AddScoped<
            ITwoFactorTokenProvider,
            TwoFactorTokenProvider>();

        services.AddScoped<
            ISignInCompletionService,
            SignInCompletionService>();

        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<
            IValidator<TwoFactorSetupRequest>,
            TwoFactorSetupRequestValidator>();

        services.AddScoped<
            IValidator<TwoFactorEnableRequest>,
            TwoFactorEnableRequestValidator>();

        services.AddScoped<
            IValidator<TwoFactorChallengeRequest>,
            TwoFactorChallengeRequestValidator>();

        services.AddScoped<
            IValidator<TwoFactorManagementRequest>,
            TwoFactorManagementRequestValidator>();


        return services;
    }

    private static bool HasValidCodeHashKey(PasswordResetOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.CodeHashKey))
        {
            return false;
        }

        try
        {
            return Convert.FromBase64String(options.CodeHashKey).Length >= 32;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
