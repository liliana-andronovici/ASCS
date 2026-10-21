using FluentValidation;
using FluentValidation.Results;

namespace InfigoHR.Authentication.Exceptions;

public class PasswordResetException : ValidationException
{
    private PasswordResetException(string propertyName, string message)
        : base(new[]
        {
            new ValidationFailure(propertyName, message)
        })
    {
    }

    public static PasswordResetException InvalidCode()
    {
        return new PasswordResetException(
            "Code",
            "The code is invalid, expired, or unavailable. Request another code.");
    }

    public static PasswordResetException InvalidToken()
    {
        return new PasswordResetException(
            "ResetToken",
            "The reset credential is invalid or expired. Start again.");
    }
}