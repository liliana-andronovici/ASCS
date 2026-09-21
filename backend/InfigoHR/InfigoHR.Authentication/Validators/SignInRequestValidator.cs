using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress()
            .Must(email => !email.Any(char.IsWhiteSpace))
            .WithMessage("Email must not contain whitespace.");

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}