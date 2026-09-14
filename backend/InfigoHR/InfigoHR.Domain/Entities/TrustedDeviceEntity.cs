using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class TrustedDeviceEntity : BaseEntity
{
    public string DeviceToken { get; set; }
    public DateTime TrustedUntilUtc { get; set; }

    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
}
