using FluentValidation;
using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Application.Validators.Roles;

public class UpdateRoleModelValidator : AbstractValidator<UpdateRoleModel>
{
    public UpdateRoleModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
