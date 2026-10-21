namespace InfigoHR.Application.Models.Employees;

public class EmployeeEquipmentModel
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public DateTime AssignedOnUtc { get; set; }
    public DateTime? ReturnedOnUtc { get; set; }
}
