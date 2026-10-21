using FluentValidation;
using InfigoHR.Application.Models.Permissions;

namespace InfigoHR.Application.Validators.Permissions;

public class CreatePermissionModelValidator : AbstractValidator<CreatePermissionModel>
{
    public CreatePermissionModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
