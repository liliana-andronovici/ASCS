using System.Text.RegularExpressions;

namespace InfigoHR.Authentication.Validators;

public static class TwoFactorValidationRules
{
    public static bool IsCode(string? value) =>
        value is not null && Regex.IsMatch(value, "\\A[0-9]{6}\\z");

    public static bool IsRecoveryCode(string? value) =>
        value is not null && Regex.IsMatch(value, "\\A[0-9A-F]{32}\\z");

    public static bool HasOneProof(string? code, string? recoveryCode) =>
        (IsCode(code) && recoveryCode is null) ||
        (code is null && IsRecoveryCode(recoveryCode));
}
