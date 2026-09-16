using FluentValidation;
using InfigoHR.Application.Models.Departments;

namespace InfigoHR.Application.Validators.Departments;

// changed: FluentValidation added
public class UpdateDepartmentModelValidator : AbstractValidator<UpdateDepartmentModel>
{
    public UpdateDepartmentModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);
    }
}
