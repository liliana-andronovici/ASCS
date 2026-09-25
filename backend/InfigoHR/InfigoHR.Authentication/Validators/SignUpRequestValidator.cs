using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    private readonly IEmployeeRepository _repository;
    
    public SignUpRequestValidator(IEmployeeRepository repository)
    {
        _repository = repository;
        
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress()
            .MustAsync(BeAvailableEmailAsync)
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
    
    private async Task<bool> BeAvailableEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var emailExists = await _repository.EmailExistsAsync(
            email,
            cancellationToken);

        return !emailExists;
    }
}