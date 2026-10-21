using FluentValidation;
using InfigoHR.Application.Models.LeaveRequests;

namespace InfigoHR.Application.Validators.LeaveRequests;

public class UpdateLeaveRequestModelValidator : AbstractValidator<UpdateLeaveRequestModel>
{
    public UpdateLeaveRequestModelValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.StartDateUtc).Must(x => x.Kind == DateTimeKind.Utc).WithMessage("StartDateUtc must be UTC.");
        RuleFor(x => x.EndDateUtc).Must(x => x.Kind == DateTimeKind.Utc).WithMessage("EndDateUtc must be UTC.");
        RuleFor(x => x.StartDateUtc).LessThanOrEqualTo(x => x.EndDateUtc);
        RuleFor(x => x.Note).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
    }
}
