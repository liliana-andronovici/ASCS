namespace InfigoHR.Domain.Entities;

public sealed class RoleEntity
{
    public const int EmployeeRoleId = 1;
    public const int AdminRoleId = 2;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<PermissionEntity> Permissions { get; set; }
        = new List<PermissionEntity>();

    public ICollection<EmployeeEntity> Employees { get; set; }
        = new List<EmployeeEntity>();
}