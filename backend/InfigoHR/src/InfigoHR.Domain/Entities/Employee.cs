using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public DateTime Started { get; set; }
    public int? DepartmentId { get; set; }
    public int LocationId { get; set; }

    // Self-referencing FK: an employee's direct manager (nullable, e.g. top of the org chart has none)
    public int? ManagerId { get; set; }

    // Onboarding invite flow
    public string? InviteToken { get; set; }
    public DateTime? InviteTokenExpiresAt { get; set; }

    //status by default -> inactive
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Inactive;

    // Navigation properties
    public virtual Department? Department { get; set; }
    public virtual Location Location { get; set; } = null!;

    public virtual Employee? Manager { get; set; }
    public virtual ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

    public virtual Auth? Auth { get; set; }
    public virtual TwoFactorAuth? TwoFactorAuth { get; set; }

    public virtual ICollection<TrustedDevice> TrustedDevices { get; set; } = new List<TrustedDevice>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}