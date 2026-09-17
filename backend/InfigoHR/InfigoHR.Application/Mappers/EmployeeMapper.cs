using InfigoHR.Application.Models;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class EmployeeMapper
{
    public static EmployeeModel ToModel(EmployeeEntity entity) =>
        new(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.Title,
            entity.StartedOnUtc, entity.DepartmentId, entity.LocationId, entity.ManagerId, entity.Status);

    public static EmployeeEntity ToEntity(CreateEmployeeModel model) =>
        new()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Title = model.Title,
            StartedOnUtc = model.StartedOnUtc,
            DepartmentId = model.DepartmentId,
            LocationId = model.LocationId,
            ManagerId = model.ManagerId,
            Status = model.Status
        };

    public static void ToEntity(UpdateEmployeeModel model, EmployeeEntity entity)
    {
        entity.FirstName = model.FirstName;
        entity.LastName = model.LastName;
        entity.Email = model.Email;
        entity.Title = model.Title;
        entity.StartedOnUtc = model.StartedOnUtc;
        entity.DepartmentId = model.DepartmentId;
        entity.LocationId = model.LocationId;
        entity.ManagerId = model.ManagerId;
        entity.Status = model.Status;
    }
}