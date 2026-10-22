using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class AuthEntity : BaseEntity
{
    public string PasswordHash { get; set; } = string.Empty;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiresAtUtc { get; set; }

    public string? ResetCodeHash { get; set; }
    public DateTime? ResetCodeExpiresAtUtc { get; set; }
    public DateTime? ResetRequestedAtUtc { get; set; }
    public int ResetAttempts { get; set; }

    public Guid TokenVersion { get; set; } = Guid.NewGuid();
    public Guid ConcurrencyVersion { get; set; } = Guid.NewGuid();

    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
}