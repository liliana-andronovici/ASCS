using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class RoleEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<PermissionEntity> Permissions { get; set; }
        = new List<PermissionEntity>();

    public virtual ICollection<EmployeeEntity> Employees { get; set; }
        = new List<EmployeeEntity>();
}