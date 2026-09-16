using FluentValidation;
using InfigoHR.Application.Models;

namespace InfigoHR.Application.Validators;

public class CreateEmployeeModelValidator : AbstractValidator<CreateEmployeeModel>
{
    public CreateEmployeeModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.StartedOnUtc).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}