using System.Text;
using InfigoHR.Authentication.Jwt;
using InfigoHR.Authentication.Services;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InfigoHR.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "Jwt:Issuer is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "Jwt:Audience is required.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.SecretKey) &&
                    Encoding.UTF8.GetByteCount(options.SecretKey) >= 32,
                "Jwt:SecretKey must contain at least 32 bytes.")
            .Validate(
                options => options.ExpirationMinutes > 0,
                "Jwt:ExpirationMinutes must be greater than zero.")
            .ValidateOnStart();

        services.AddScoped<JwtProvider>();
        services.AddScoped<AuthService>();
        services.AddScoped<IPasswordHasher<AuthEntity>,
            PasswordHasher<AuthEntity>>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,

                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.SecretKey)),

                        ValidAlgorithms =
                            new[] { SecurityAlgorithms.HmacSha256 },

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

        services.AddAuthorization(options =>
        {
            foreach (var permission in Enum.GetValues<Permission>())
            {
                var permissionName = permission.ToString();

                options.AddPolicy(permissionName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(
                        "permissions",
                        permissionName);
                });
            }
        });

        return services;
    }
}