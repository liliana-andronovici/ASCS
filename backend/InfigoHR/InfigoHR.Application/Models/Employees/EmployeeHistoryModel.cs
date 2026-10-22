namespace InfigoHR.Application.Models.Employees;

public class EmployeeHistoryModel
{
    public int Id { get; set; }
    public int? ActorEmployeeId { get; set; }
    public string[] ChangedFields { get; set; } = [];
    public DateTime CreatedOnUtc { get; set; }
}