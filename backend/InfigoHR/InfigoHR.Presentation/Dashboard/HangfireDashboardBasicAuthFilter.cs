using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InfigoHR.Presentation.Dashboard;

public sealed class HangfireDashboardBasicAuthFilter(IConfiguration configuration) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // A local HTTP connection is permitted for development; remote connections require HTTPS.
        if (!httpContext.Request.IsHttps && !IsLoopback(httpContext.Connection.RemoteIpAddress))
        {
            return false;
        }

        var expectedUsername = configuration["HangfireDashboard:Username"];
        var expectedPassword = configuration["HangfireDashboard:Password"];
        if (string.IsNullOrWhiteSpace(expectedUsername) || string.IsNullOrEmpty(expectedPassword))
        {
            return false;
        }

        if (!AuthenticationHeaderValue.TryParse(httpContext.Request.Headers["Authorization"].ToString(), out var header) ||
            header is null ||
            !string.Equals(header.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrEmpty(header.Parameter))
        {
            return Challenge(httpContext);
        }

        string credentials;
        try
        {
            credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter));
        }
        catch (FormatException)
        {
            return Challenge(httpContext);
        }

        var separator = credentials.IndexOf(':');
        if (separator < 0 ||
            !Matches(credentials[..separator], expectedUsername) ||
            !Matches(credentials[(separator + 1)..], expectedPassword))
        {
            return Challenge(httpContext);
        }

        return true;
    }

    private static bool IsLoopback(IPAddress? address) =>
        address is not null &&
        (IPAddress.IsLoopback(address) ||
         (address.IsIPv4MappedToIPv6 && IPAddress.IsLoopback(address.MapToIPv4())));

    private static bool Matches(string candidate, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(candidate)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));

    private static bool Challenge(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"InfigoHR Hangfire\", charset=\"UTF-8\"";
        return false;
    }
}
