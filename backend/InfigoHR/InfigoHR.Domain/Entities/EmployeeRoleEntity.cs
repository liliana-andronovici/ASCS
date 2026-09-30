namespace InfigoHR.Domain.Entities;

public class EmployeeRoleEntity
{
    public int EmployeeId { get; set; }
    public int RoleId { get; set; }

    public virtual EmployeeEntity Employee { get; set; } = null!;
    public virtual RoleEntity Role { get; set; } = null!;
}