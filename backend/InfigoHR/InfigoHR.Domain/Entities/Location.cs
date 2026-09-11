using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class Location : BaseEntity
{
    public required string Name { get; set; }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
