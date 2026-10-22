using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class CompletePasswordResetRequestValidator
    : AbstractValidator<CompletePasswordResetRequest>
{
    public CompletePasswordResetRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress();

        RuleFor(request => request.ResetToken)
            .NotEmpty()
            .Matches("^[A-F0-9]{64}$")
            .WithMessage("Invalid reset credential.");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .MinimumLength(12)
            .MaximumLength(128);

        RuleFor(request => request.ConfirmPassword)
            .NotEmpty()
            .Equal(request => request.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}