using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class DepartmentEntity : BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
}
