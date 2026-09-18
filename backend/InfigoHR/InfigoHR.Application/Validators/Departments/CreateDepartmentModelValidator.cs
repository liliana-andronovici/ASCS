using FluentValidation;
using InfigoHR.Application.Models.Departments;

namespace InfigoHR.Application.Validators.Departments;

public class CreateDepartmentModelValidator : AbstractValidator<CreateDepartmentModel>
{
    public CreateDepartmentModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);
    }
}
