using System.Threading.RateLimiting;
using InfigoHR.Authentication.Options;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace InfigoHR.Presentation.Configurations;

public class PasswordResetRateLimiterSetup
    : IConfigureOptions<RateLimiterOptions>
{
    private readonly PasswordResetOptions _options;

    public PasswordResetRateLimiterSetup(
        IOptions<PasswordResetOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(RateLimiterOptions options)
    {
        options.RejectionStatusCode =
            StatusCodes.Status429TooManyRequests;

        options.AddPolicy(
            "password-reset",
            context =>
            {
                var address = context.Connection.RemoteIpAddress?
                    .ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    address,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = _options.RequestsPerMinute,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
    }
}