using FluentValidation;
using InfigoHR.Application.Models.Employees;

namespace InfigoHR.Application.Validators.Employees;

public class CreateEmployeeEquipmentModelValidator : AbstractValidator<CreateEmployeeEquipmentModel>
{
    public CreateEmployeeEquipmentModelValidator()
    {
        RuleFor(model => model.EquipmentId).GreaterThan(0);
        RuleFor(model => model.AssignedOn).NotEmpty();
    }
}
