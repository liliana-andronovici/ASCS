using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class AuthEntity : BaseEntity
{
    public string PasswordHash { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiresAtUtc { get; set; }

    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
}
