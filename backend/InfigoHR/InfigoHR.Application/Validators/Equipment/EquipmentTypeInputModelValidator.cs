using FluentValidation;
using InfigoHR.Application.Models.Equipment;

namespace InfigoHR.Application.Validators.Equipment;

public class EquipmentTypeInputModelValidator : AbstractValidator<EquipmentTypeInputModel>
{
    public EquipmentTypeInputModelValidator()
    {
        RuleFor(model => model.Name).Must(value => !string.IsNullOrWhiteSpace(value)).MaximumLength(80);
    }
}
