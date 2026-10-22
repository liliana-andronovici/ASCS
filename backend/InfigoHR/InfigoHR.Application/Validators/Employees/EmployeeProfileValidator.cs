using FluentValidation;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Validators.Employees;

public class EmployeeProfileValidator : AbstractValidator<EmployeeEntity>
{
    public EmployeeProfileValidator()
    {
        RuleFor(employee => employee.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(employee => employee.LastName).NotEmpty().MaximumLength(100);
        RuleFor(employee => employee.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(employee => employee.Title).NotEmpty().MaximumLength(150);
        RuleFor(employee => employee.StartedOnUtc)
            .NotEmpty()
            .Must(value => value.Kind == DateTimeKind.Utc)
            .WithMessage("StartedOnUtc must be UTC (use a Z suffix).");
        RuleFor(employee => employee.Status).IsInEnum();
        RuleFor(employee => employee.Phone).MaximumLength(40);
        RuleFor(employee => employee.Desk).MaximumLength(150);
        RuleFor(employee => employee.EmployeeCode).MaximumLength(50);
        RuleFor(employee => employee.EmploymentType).MaximumLength(50);
        RuleFor(employee => employee.ContractType).MaximumLength(100);
        RuleFor(employee => employee.WeeklyHours)
            .InclusiveBetween(0, 168)
            .Must(hours => hours is null || decimal.Round(hours.Value, 2) == hours.Value)
            .WithMessage("WeeklyHours must be between 0 and 168 with at most two decimal places.")
            .When(employee => employee.WeeklyHours.HasValue);
        RuleFor(employee => employee.Languages).NotNull().Must(languages => languages is { Length: <= 20 });
        RuleForEach(employee => employee.Languages).NotEmpty().MaximumLength(60);
        RuleFor(employee => employee)
            .Must(employee =>
                (employee.BirthdayMonth is null && employee.BirthdayDay is null) ||
                (employee.BirthdayMonth is >= 1 and <= 12 && employee.BirthdayDay is >= 1 &&
                 employee.BirthdayDay <= DateTime.DaysInMonth(2000, employee.BirthdayMonth.Value)))
            .WithMessage("Birthday month and day must form a valid date, or both be null.");
    }
}