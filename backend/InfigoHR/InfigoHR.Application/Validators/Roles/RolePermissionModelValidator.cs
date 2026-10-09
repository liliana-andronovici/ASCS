using FluentValidation;
using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Application.Validators.Roles;

public class RolePermissionModelValidator : AbstractValidator<RolePermissionModel>
{
    public RolePermissionModelValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Role id is required.");
        RuleFor(x => x)
            .Must(x => x.PermissionId > 0 || !string.IsNullOrWhiteSpace(x.PermissionName))
            .WithMessage("Permission id or permission name is required.");
    }
}
