using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    public SignUpRequestValidator(IEmployeeRepository repository)
    {
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress()
            .Must(email => !email.Any(char.IsWhiteSpace))
            .WithMessage("Email must not contain whitespace.")
            .MustAsync(async (email, cancellationToken) =>
            {
                var emailExists = await repository.EmailExistsAsync(
                    email,
                    cancellationToken);

                return !emailExists;
            })
            .WithMessage("An account with this email already exists.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(12)
            .MaximumLength(128);

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(request => request.StartedOn)
            .NotNull();
    }
}