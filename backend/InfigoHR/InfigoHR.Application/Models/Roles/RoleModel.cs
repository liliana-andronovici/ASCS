namespace InfigoHR.Application.Models.Roles;

public class RoleModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}
