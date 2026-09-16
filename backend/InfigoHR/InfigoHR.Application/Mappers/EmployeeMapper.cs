using InfigoHR.Application.Models;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class EmployeeMapper
{
    public static EmployeeModel ToDto(EmployeeEntity entity) =>
        new(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.Title,
            entity.StartedOnUtc, entity.DepartmentId, entity.LocationId, entity.ManagerId, entity.Status);
}