using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Options;
using Microsoft.Extensions.Options;

namespace InfigoHR.Authentication.Services;

public class PasswordResetTokenProvider : IPasswordResetTokenProvider
{
    private readonly byte[] _codeHashKey;

    public PasswordResetTokenProvider(
        IOptions<PasswordResetOptions> options)
    {
        _codeHashKey = Convert.FromBase64String(
            options.Value.CodeHashKey);
    }

    public string GenerateCode()
    {
        return RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6", CultureInfo.InvariantCulture);
    }

    public string GenerateToken()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));
    }

    public string HashCode(int employeeId, string code)
    {
        var value = FormattableString.Invariant(
            $"password-reset:{employeeId}:{code}");

        var hash = HMACSHA256.HashData(
            _codeHashKey,
            Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(hash);
    }

    public string HashToken(string token)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }

    public bool Matches(string expectedHash, string actualHash)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedHash),
            Encoding.UTF8.GetBytes(actualHash));
    }
}
