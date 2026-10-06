using FluentValidation;
using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Application.Validators.Roles;

public class SetRolePermissionsModelValidator : AbstractValidator<SetRolePermissionsModel>
{
    public SetRolePermissionsModelValidator()
    {
        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions list is required.");
        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Permission name must not be empty.");
    }
}
