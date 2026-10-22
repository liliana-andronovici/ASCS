using InfigoHR.Authentication.Models;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Authentication.Mappers;

public static class TwoFactorMapper
{
    public static AuthResponse Authenticated(string accessToken) => new()
    {
        AccessToken = accessToken
    };

    public static AuthResponse Challenge(string token, DateTime expiresAt) => new()
    {
        RequiresTwoFactor = true,
        ChallengeToken = token,
        ChallengeExpiresAtUtc = expiresAt
    };

    public static TwoFactorStatusResponse Status(TwoFactorAuthEntity? state) => new()
    {
        IsEnabled = state?.IsEnabled == true,
        RecoveryCodesRemaining = state?.IsEnabled == true
            ? state.RecoveryCodeHashes.Length : 0
    };

    public static TwoFactorSetupResponse Setup(TwoFactorAuthEntity state,
        string sharedKey, string uri) => new()
    {
        SetupId = state.SetupId,
        SharedKey = sharedKey,
        AuthenticatorUri = uri,
        ExpiresAtUtc = state.SetupExpiresAtUtc!.Value
    };

    public static TwoFactorRecoveryResponse Recovery(string accessToken,
        string[] codes) => new()
    {
        AccessToken = accessToken,
        RecoveryCodes = codes
    };
}
