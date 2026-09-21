using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class PermissionEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}