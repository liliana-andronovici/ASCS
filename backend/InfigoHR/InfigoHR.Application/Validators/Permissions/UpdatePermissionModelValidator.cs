using FluentValidation;
using InfigoHR.Application.Models.Permissions;

namespace InfigoHR.Application.Validators.Permissions;

public class UpdatePermissionModelValidator : AbstractValidator<UpdatePermissionModel>
{
    public UpdatePermissionModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
