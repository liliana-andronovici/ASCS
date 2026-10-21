using FluentValidation;
using InfigoHR.Application.Models.Equipment;

namespace InfigoHR.Application.Validators.Equipment;

public class EquipmentInputModelValidator : AbstractValidator<EquipmentInputModel>
{
    public EquipmentInputModelValidator()
    {
        RuleFor(model => model.Name).Must(value => !string.IsNullOrWhiteSpace(value)).MaximumLength(150);
        RuleFor(model => model.Code).Must(value => !string.IsNullOrWhiteSpace(value)).MaximumLength(80);
        RuleFor(model => model.EquipmentTypeId).GreaterThan(0);
    }
}
