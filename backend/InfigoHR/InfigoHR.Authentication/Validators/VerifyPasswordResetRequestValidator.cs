using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class VerifyPasswordResetRequestValidator
    : AbstractValidator<VerifyPasswordResetRequest>
{
    public VerifyPasswordResetRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress();

        RuleFor(request => request.Code)
            .NotEmpty()
            .Matches("^[0-9]{6}$")
            .WithMessage("Enter the 6-digit code.");
    }
}