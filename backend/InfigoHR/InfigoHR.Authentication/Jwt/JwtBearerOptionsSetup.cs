using System.Text;
using System.IdentityModel.Tokens.Jwt;
using InfigoHR.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InfigoHR.Authentication.Jwt;

public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options;

    public JwtBearerOptionsSetup(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "role",

            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,

            ValidateAudience = true,
            ValidAudience = _options.Audience,

            ValidateLifetime = true,
            RequireExpirationTime = true,

            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)),

            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = ValidateSessionAsync
        };
    }

    private static async Task ValidateSessionAsync(
        TokenValidatedContext context)
    {
        var employeeIdClaim = context.Principal?
            .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        var tokenVersionClaim = context.Principal?
            .FindFirst("token_version")?.Value;

        if (!int.TryParse(employeeIdClaim, out var employeeId) ||
            !Guid.TryParse(tokenVersionClaim, out var tokenVersion))
        {
            context.Fail("Invalid session.");
            return;
        }

        var repository = context.HttpContext.RequestServices
            .GetRequiredService<IAuthRepository>();

        var isSessionValid = await repository.IsSessionValidAsync(
            employeeId,
            tokenVersion,
            context.HttpContext.RequestAborted);

        if (!isSessionValid)
        {
            context.Fail("Invalid session.");
        }
    }
}
