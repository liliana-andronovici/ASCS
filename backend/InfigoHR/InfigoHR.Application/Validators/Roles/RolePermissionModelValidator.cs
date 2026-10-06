using FluentValidation;
using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Application.Validators.Roles;

public class RolePermissionModelValidator : AbstractValidator<RolePermissionModel>
{
    public RolePermissionModelValidator()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Permission name is required.");
    }
}
