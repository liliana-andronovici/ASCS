using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress()
            .Must(EmailValidationRules.NotContainWhitespace)
            .WithMessage("Email must not contain whitespace.");

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}