using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class EquipmentEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int EquipmentTypeId { get; set; }
    public EquipmentTypeEntity EquipmentType { get; set; } = null!;
}
