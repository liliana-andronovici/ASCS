using InfigoHR.Domain.Common;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class Employee : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Title { get; set; }
    public DateTime StartedOnUtc { get; set; }
    public int? DepartmentId { get; set; }
    public int LocationId { get; set; }
    public int? ManagerId { get; set; }
    public string? InviteToken { get; set; }
    public DateTime? InviteTokenExpiresAtUtc { get; set; }
    public EmployeeStatus Status { get; set; }
    public virtual Department? Department { get; set; }
    public virtual Location Location { get; set; } = null!;
    public virtual Employee? Manager { get; set; }
    public virtual Auth Auth { get; set; } = null!;
    public virtual TwoFactorAuth? TwoFactorAuth { get; set; }
    public virtual ICollection<TrustedDevice> TrustedDevices { get; set; } = new List<TrustedDevice>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
