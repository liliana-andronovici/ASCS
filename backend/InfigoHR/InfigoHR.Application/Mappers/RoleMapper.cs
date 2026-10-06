using InfigoHR.Application.Models.Roles;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class RoleMapper
{
    public static RoleModel ToModel(RoleEntity entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Permissions = entity.Permissions.Select(p => p.Name).OrderBy(n => n).ToList()
        };

    public static RoleEntity ToEntity(CreateRoleModel model) =>
        new()
        {
            Name = model.Name
        };

    public static void ToEntity(UpdateRoleModel model, RoleEntity entity)
    {
        entity.Name = model.Name;
    }
}
