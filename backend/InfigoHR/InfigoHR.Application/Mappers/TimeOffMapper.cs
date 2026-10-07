using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Mappers;

public static class TimeOffMapper
{
    public static TimeOffModel ToModel(TimeOffEntity entity) => new()
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

    public static TimeOffEntity ToEntity(CreateTimeOffModel model, int employeeId) => new()
    {
        StartDateUtc = model.StartDateUtc,
        EndDateUtc = model.EndDateUtc,
        Note = model.Note,
        Type = model.Type,
        Status = TimeOffStatus.Pending,
        EmployeeId = employeeId
    };

    public static void Apply(UpdateTimeOffModel model, TimeOffEntity entity)
    {
        entity.StartDateUtc = model.StartDateUtc;
        entity.EndDateUtc = model.EndDateUtc;
        entity.Note = model.Note;
        entity.Type = model.Type;
    }
}
