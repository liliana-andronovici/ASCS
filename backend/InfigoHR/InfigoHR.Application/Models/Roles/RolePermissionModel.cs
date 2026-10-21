namespace InfigoHR.Application.Models.Roles;

public class RolePermissionModel
{
    public int RoleId { get; set; }
    public int? PermissionId { get; set; }
    public string? PermissionName { get; set; } = string.Empty;
}
