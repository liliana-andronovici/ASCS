using System.Threading.RateLimiting;
using InfigoHR.Authentication.Options;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace InfigoHR.Presentation.Configurations;

public class TwoFactorRateLimiterSetup
    : IConfigureOptions<RateLimiterOptions>
{
    private readonly TwoFactorOptions _options;

    public TwoFactorRateLimiterSetup(
        IOptions<TwoFactorOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(RateLimiterOptions options)
    {
        options.RejectionStatusCode =
            StatusCodes.Status429TooManyRequests;

        options.AddPolicy(
            "two-factor",
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
