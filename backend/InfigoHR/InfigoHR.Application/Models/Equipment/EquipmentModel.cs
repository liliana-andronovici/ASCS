namespace InfigoHR.Application.Models.Equipment;

public class EquipmentModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int EquipmentTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}
