using FluentValidation;
using InfigoHR.Application.DTOs;

namespace InfigoHR.Application.Validators;

public class CreateLocationModelValidator : AbstractValidator<CreateLocationModel>
{
    public CreateLocationModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
    }
}