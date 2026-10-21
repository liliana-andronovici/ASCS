using FluentValidation;
using FluentValidation.Results;

namespace InfigoHR.Authentication.Exceptions;

public class TwoFactorException : ValidationException
{
    private TwoFactorException(string message)
        : base(new[] { new ValidationFailure("TwoFactor", message) })
    {
    }

    public static TwoFactorException Invalid() => new(
        "The credential is invalid, expired, already used, or temporarily unavailable.");

    public static TwoFactorException Conflict() => new(
        "Authentication state changed. Sign in again and retry.");

    public static TwoFactorException AlreadyEnabled() => new(
        "Two-factor authentication is already enabled.");
}
