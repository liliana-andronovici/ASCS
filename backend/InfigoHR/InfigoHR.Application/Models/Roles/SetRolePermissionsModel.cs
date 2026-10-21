namespace InfigoHR.Application.Models.Roles;

public class SetRolePermissionsModel
{
    public int RoleId { get; set; }
    public List<string> Permissions { get; set; } = new();
}
