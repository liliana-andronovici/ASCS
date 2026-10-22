using InfigoHR.Application.Models.Employees;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class EmployeeMapper
{
    public static EmployeeModel ToModel(EmployeeEntity entity) =>
        new()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Title = entity.Title,
            StartedOnUtc = entity.StartedOnUtc,
            DepartmentId = entity.DepartmentId,
            LocationId = entity.LocationId,
            ManagerId = entity.ManagerId,
            Status = entity.Status,
            DepartmentName = entity.Department?.Name,
            LocationName = entity.Location?.Name,
            Manager = entity.Manager is null ? null : new EmployeeMainDataModel
            {
                Id = entity.Manager.Id,
                FirstName = entity.Manager.FirstName,
                LastName = entity.Manager.LastName,
                Title = entity.Manager.Title
            }
        };

    public static EmployeeProfileModel ToProfileModel(EmployeeEntity entity) => new()
    {
        Id = entity.Id,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        Title = entity.Title,
        StartedOnUtc = entity.StartedOnUtc,
        DepartmentId = entity.DepartmentId,
        DepartmentName = entity.Department?.Name,
        LocationId = entity.LocationId,
        LocationName = entity.Location?.Name,
        ManagerId = entity.ManagerId,
        Manager = entity.Manager is null ? null : new EmployeeMainDataModel
        {
            Id = entity.Manager.Id,
            FirstName = entity.Manager.FirstName,
            LastName = entity.Manager.LastName,
            Title = entity.Manager.Title
        },
        Status = entity.Status,
        Phone = entity.Phone,
        Desk = entity.Desk,
        BirthdayMonth = entity.BirthdayMonth,
        BirthdayDay = entity.BirthdayDay,
        Languages = entity.Languages.ToArray(),
        EmployeeCode = entity.EmployeeCode,
        EmploymentType = entity.EmploymentType,
        ContractType = entity.ContractType,
        WeeklyHours = entity.WeeklyHours,
        ProfileVersion = entity.ProfileVersion
    };

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
    
    public static OrgChartEmployeeModel ToOrgChartModel(EmployeeEntity entity) =>
        new()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Title = entity.Title
        };
}
