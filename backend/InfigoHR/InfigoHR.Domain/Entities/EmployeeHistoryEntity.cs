using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class EmployeeHistoryEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? ActorEmployeeId { get; set; }
    public string[] ChangedFields { get; set; } = [];
}