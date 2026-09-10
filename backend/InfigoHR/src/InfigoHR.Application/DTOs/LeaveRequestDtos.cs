using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.DTOs;

public sealed record LeaveRequestDto(
    int Id,
    int EmployeeId,
    DateTime StartDate,
    DateTime EndDate,
    string Note,
    LeaveType Type,
    LeaveRequestStatus Status);

public sealed record CreateLeaveRequestDto(
    int EmployeeId,
    DateTime StartDate,
    DateTime EndDate,
    string Note,
    LeaveType Type,
    LeaveRequestStatus Status = LeaveRequestStatus.Pending);

public sealed record UpdateLeaveRequestDto(
    int EmployeeId,
    DateTime StartDate,
    DateTime EndDate,
    string Note,
    LeaveType Type,
    LeaveRequestStatus Status);
