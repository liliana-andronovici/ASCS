using FluentValidation;
using InfigoHR.Application.DTOs;

namespace InfigoHR.Application.Validators;

public class UpdateLocationModelValidator : AbstractValidator<UpdateLocationModel>
{
    public UpdateLocationModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
    }
}