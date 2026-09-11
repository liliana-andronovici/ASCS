using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class TrustedDevice : BaseEntity
{
    public required string DeviceToken { get; set; }
    public int EmployeeId { get; set; }
    public DateTime TrustedUntilUtc { get; set; }
    public virtual Employee Employee { get; set; } = null!;
}
