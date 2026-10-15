using InfigoHR.Application.Models.LeaveRequests;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Mappers;

public static class LeaveRequestMapper
{
    public static LeaveRequestModel ToModel(LeaveRequestEntity entity) => new()
    {
        Id = entity.Id,
        StartDateUtc = entity.StartDateUtc,
        EndDateUtc = entity.EndDateUtc,
        Note = entity.Note,
        Type = entity.Type,
        Status = entity.Status,
        CreatedOnUtc = entity.CreatedOnUtc,
        UpdatedOnUtc = entity.UpdatedOnUtc
    };

    public static LeaveRequestEntity ToEntity(CreateLeaveRequestModel model, int employeeId) => new()
    {
        StartDateUtc = model.StartDateUtc,
        EndDateUtc = model.EndDateUtc,
        Note = model.Note,
        Type = model.Type,
        Status = LeaveRequestStatus.Pending,
        EmployeeId = employeeId
    };

    public static void ToEntity(UpdateLeaveRequestModel model, LeaveRequestEntity entity)
    {
        entity.StartDateUtc = model.StartDateUtc;
        entity.EndDateUtc = model.EndDateUtc;
        entity.Note = model.Note;
        entity.Type = model.Type;
    }
}
