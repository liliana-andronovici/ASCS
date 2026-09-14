using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class TwoFactorAuthEntity : BaseEntity
{
    public string SecretKey { get; set; }

    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
}
