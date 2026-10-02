using InfigoHR.Domain.Common;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class OnboardingTaskEntity : BaseEntity
{
    public int OnboardingId { get; set; }
    public virtual OnboardingEntity? Onboarding { get; set; }
    public OnboardingSection Section { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public DateTime? CompletedOnUtc { get; set; }
}