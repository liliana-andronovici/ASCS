using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class EmployeeEquipmentEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public int EquipmentId { get; set; }
    public EquipmentEntity Equipment { get; set; } = null!;
    public DateTime AssignedOnUtc { get; set; }
    public DateTime? ReturnedOnUtc { get; set; }
}
