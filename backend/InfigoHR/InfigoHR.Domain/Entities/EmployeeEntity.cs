using InfigoHR.Domain.Common;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class EmployeeEntity : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public DateTime StartedOnUtc { get; set; }
    public string? InviteToken { get; set; }
    public DateTime? InviteTokenExpiresAtUtc { get; set; }
    public EmployeeStatus Status { get; set; }

    public int? DepartmentId { get; set; }
    public virtual DepartmentEntity? Department { get; set; }

    public int? LocationId { get; set; }
    public virtual LocationEntity? Location { get; set; }

    public int? ManagerId { get; set; }
    public virtual EmployeeEntity? Manager { get; set; }

    public virtual AuthEntity? Auth { get; set; }
    public virtual TwoFactorAuthEntity? TwoFactorAuth { get; set; }
    public virtual ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
    public virtual ICollection<TrustedDeviceEntity> TrustedDevices { get; set; } = new List<TrustedDeviceEntity>();
    public virtual ICollection<LeaveRequestEntity> LeaveRequests { get; set; } = new List<LeaveRequestEntity>();
    
    public virtual ICollection<RoomBookingEntity> RoomBookings { get; set; } = new List<RoomBookingEntity>();
}
