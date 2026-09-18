using FluentValidation;
using InfigoHR.Application.Models.Departments;

namespace InfigoHR.Application.Validators.Departments;

public class UpdateDepartmentModelValidator : AbstractValidator<UpdateDepartmentModel>
{
    public UpdateDepartmentModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);
    }
}
