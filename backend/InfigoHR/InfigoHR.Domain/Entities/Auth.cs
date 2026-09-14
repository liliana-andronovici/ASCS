using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class AuthEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public required string PasswordHash { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiresAtUtc { get; set; }
    public virtual Employee Employee { get; set; } = null!;
}
