using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using OtpNet;

namespace InfigoHR.Authentication.Services;

public class TwoFactorTokenProvider : ITwoFactorTokenProvider
{
    private readonly IDataProtectionProvider _protection;
    private readonly TwoFactorOptions _options;

    public TwoFactorTokenProvider(IDataProtectionProvider protection,
        IOptions<TwoFactorOptions> options)
    {
        _protection = protection;
        _options = options.Value;
    }

    public string GenerateSecret() => Base32Encoding.ToString(RandomNumberGenerator.GetBytes(20));

    private IDataProtector Protector(int employeeId) =>
        _protection.CreateProtector(
            "InfigoHR.TwoFactor.Secret.v1",
            employeeId.ToString(CultureInfo.InvariantCulture));

    public string ProtectSecret(int employeeId, string secret) =>
        Protector(employeeId).Protect(secret);

    public bool TryVerifyCode(int employeeId, string protectedSecret,
        string code, DateTime now, out long timeStep)
    {
        timeStep = 0;

        try
        {
            var secret = Protector(employeeId).Unprotect(protectedSecret);
            var totp = new Totp(Base32Encoding.ToBytes(secret),
                step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.VerifyTotp(now, code, out timeStep,
                new VerificationWindow(previous: 1, future: 1));
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

    public string GetAuthenticatorUri(string email, string secret)
    {
        var issuer = Uri.EscapeDataString(_options.Issuer);
        var account = Uri.EscapeDataString(email);
        return $"otpauth://totp/{issuer}:{account}?secret={secret}" +
               $"&issuer={issuer}&algorithm=SHA1&digits=6&period=30";
    }

    public string GenerateChallenge() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    public string HashChallenge(string token) => Hash("challenge:" + token);

    public string[] GenerateRecoveryCodes() =>
        Enumerable.Range(0, _options.RecoveryCodeCount)
            .Select(_ => Convert.ToHexString(RandomNumberGenerator.GetBytes(16)))
            .ToArray();

    public string HashRecoveryCode(int employeeId, string code) =>
        Hash($"recovery:{employeeId.ToString(CultureInfo.InvariantCulture)}:{code}");

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
