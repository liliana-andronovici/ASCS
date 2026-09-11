using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class TwoFactorAuth : BaseEntity
{
    public int EmployeeId { get; set; }
    public required string SecretKey { get; set; }
    public virtual Employee Employee { get; set; } = null!;
}
