using FluentValidation;
using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Validators.TimeOff;

public class CreateTimeOffModelValidator : AbstractValidator<CreateTimeOffModel>
{
    public CreateTimeOffModelValidator()
    {
        RuleFor(x => x.StartDateUtc).Must(x => x.Kind == DateTimeKind.Utc).WithMessage("StartDateUtc must be UTC.");
        RuleFor(x => x.EndDateUtc).Must(x => x.Kind == DateTimeKind.Utc).WithMessage("EndDateUtc must be UTC.");
        RuleFor(x => x.StartDateUtc).LessThanOrEqualTo(x => x.EndDateUtc);
        RuleFor(x => x.Note).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
    }
}
