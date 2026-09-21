namespace InfigoHR.Domain.Entities;

public class RolePermissionEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public virtual RoleEntity Role { get; set; } = null!;
    public virtual PermissionEntity Permission { get; set; } = null!;
}