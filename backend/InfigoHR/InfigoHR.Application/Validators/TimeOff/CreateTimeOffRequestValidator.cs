using FluentValidation;
using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Validators.TimeOff;

public class CreateTimeOffRequestValidator : AbstractValidator<CreateTimeOffRequestModel>
{
    public CreateTimeOffRequestValidator()
    {
        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date can't be in the past.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");
    }
}