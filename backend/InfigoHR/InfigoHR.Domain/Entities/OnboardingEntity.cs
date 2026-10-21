using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class OnboardingEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
    public int? BuddyId { get; set; }
    public virtual EmployeeEntity? Buddy { get; set; }
    public virtual ICollection<OnboardingTaskEntity> Tasks { get; set; } = new List<OnboardingTaskEntity>();
}