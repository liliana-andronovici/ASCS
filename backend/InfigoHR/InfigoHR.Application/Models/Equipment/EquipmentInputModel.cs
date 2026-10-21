namespace InfigoHR.Application.Models.Equipment;

public class EquipmentInputModel
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int EquipmentTypeId { get; set; }
}
