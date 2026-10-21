using InfigoHR.Application.Models.Permissions;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class PermissionMapper
{
    public static PermissionModel ToModel(PermissionEntity entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

    public static PermissionEntity ToEntity(CreatePermissionModel model) =>
        new()
        {
            Name = model.Name
        };

    public static void ToEntity(UpdatePermissionModel model, PermissionEntity entity)
    {
        entity.Name = model.Name;
    }
}
