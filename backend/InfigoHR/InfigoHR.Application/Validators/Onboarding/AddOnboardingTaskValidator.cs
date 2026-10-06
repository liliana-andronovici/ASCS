using FluentValidation;
using InfigoHR.Application.Models;

namespace InfigoHR.Application.Validators.Onboarding;

public class AddOnboardingTaskValidator : AbstractValidator<AddOnboardingTaskModel>
{
    private static readonly string[] ValidKeys = ["beforeDayOne", "firstWeek", "firstMonth", "other"];

    public AddOnboardingTaskValidator()
    {
        RuleFor(x => x.SectionKey)
            .Must(key => ValidKeys.Contains(key))
            .WithMessage("Invalid section.");
        RuleFor(x => x.DueDate)
            .Must(d => string.IsNullOrWhiteSpace(d) || DateTime.TryParse(d, out _))
            .WithMessage("Due date must be a valid date.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}