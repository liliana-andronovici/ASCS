using InfigoHR.Application.Helpers;
using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Mappers;

public static class TimeOffMapper
{
    public static TimeOffRequestModel ToModel(LeaveRequestEntity entity, string approverName) =>
        new()
        {
            Id = entity.Id,
            Type = entity.Type,
            StartDate = entity.StartDateUtc,
            EndDate = entity.EndDateUtc,
            Days = TimeOffCalculator.WorkingDays(entity.StartDateUtc, entity.EndDateUtc),
            ApproverName = approverName,
            Status = entity.Status,
            CanWithdraw = entity.Status == LeaveRequestStatus.Pending
        };
}