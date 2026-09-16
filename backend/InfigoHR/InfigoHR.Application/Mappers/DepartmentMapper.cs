using InfigoHR.Application.Models.Departments;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class DepartmentMapper
{
    public static DepartmentModel ToModel(DepartmentEntity department)
    {
        return new DepartmentModel
        {
            Id = department.Id,
            Name = department.Name,
            CreatedOnUtc = department.CreatedOnUtc,
            UpdatedOnUtc = department.UpdatedOnUtc
        };
    }

    public static DepartmentEntity ToEntity(CreateDepartmentModel model)
    {
        return new DepartmentEntity
        {
            Name = model.Name
        };
    }

    public static void Map(UpdateDepartmentModel model, DepartmentEntity department)
    {
        department.Name = model.Name;
        department.UpdatedOnUtc = DateTime.UtcNow;
    }
}
