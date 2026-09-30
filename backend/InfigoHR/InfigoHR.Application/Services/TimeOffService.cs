using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Helpers;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Services;

public class TimeOffService : ITimeOffService
{
    private const int AnnualVacationDays = 25;

    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<CreateTimeOffRequestModel> _createValidator;

    public TimeOffService(
        ILeaveRequestRepository leaveRequestRepository,
        IEmployeeRepository employeeRepository,
        IValidator<CreateTimeOffRequestModel> createValidator)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _employeeRepository = employeeRepository;
        _createValidator = createValidator;
    }

    public async Task<TimeOffOverviewModel> GetOverviewAsync(int employeeId)
    {
        var employee = await GetEmployeeAsync(employeeId);
        var approverName = await GetApproverNameAsync(employee);

        var requests = await _leaveRequestRepository.GetForEmployeeAsync(employeeId);
        var now = DateTime.UtcNow;

        var yearRequests = requests.Where(r => r.StartDateUtc.Year == now.Year).ToList();
        var monthRequests = yearRequests.Where(r => r.StartDateUtc.Month == now.Month).ToList();

        return new TimeOffOverviewModel
        {
            PolicyName = "Standard 25-day",
            ResetsOn = new DateTime(now.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ApproverName = approverName,
            Balances =
            [
                BuildVacationBalance(yearRequests),
                BuildSickBalance(yearRequests),
                BuildRemoteBalance(monthRequests)
            ]
        };
    }

    public async Task<List<TimeOffRequestModel>> GetMyRequestsAsync(int employeeId)
    {
        var employee = await GetEmployeeAsync(employeeId);
        var approverName = await GetApproverNameAsync(employee);

        var requests = await _leaveRequestRepository.GetForEmployeeAsync(employeeId);

        return requests
            .OrderByDescending(r => r.StartDateUtc)
            .Select(r => TimeOffMapper.ToModel(r, approverName))
            .ToList();
    }

    public async Task<TimeOffPreviewModel> PreviewAsync(int employeeId, TimeOffPreviewInputModel model)
    {
        var workingDays = TimeOffCalculator.WorkingDays(model.StartDate, model.EndDate);

        int? remainingAfter = null;
        if (model.Type == LeaveType.Vacation)
        {
            var yearRequests = (await _leaveRequestRepository.GetForEmployeeAsync(employeeId))
                .Where(r => r.Type == LeaveType.Vacation && r.StartDateUtc.Year == model.StartDate.Year)
                .ToList();

            var used = yearRequests
                .Where(r => r.Status == LeaveRequestStatus.Approved)
                .Sum(r => TimeOffCalculator.WorkingDays(r.StartDateUtc, r.EndDateUtc));
            var pending = yearRequests
                .Where(r => r.Status == LeaveRequestStatus.Pending)
                .Sum(r => TimeOffCalculator.WorkingDays(r.StartDateUtc, r.EndDateUtc));

            remainingAfter = AnnualVacationDays - used - pending - workingDays;
        }

        var employee = await GetEmployeeAsync(employeeId);
        var overlapping = await _leaveRequestRepository.GetOverlappingAsync(
            employeeId, employee.DepartmentId, model.StartDate, model.EndDate);

        return new TimeOffPreviewModel
        {
            WorkingDays = workingDays,
            RemainingAfter = remainingAfter,
            OverlappingPeople = overlapping
                .Where(r => r.Employee is not null)
                .Select(r => $"{r.Employee!.FirstName} {r.Employee!.LastName}")
                .Distinct()
                .ToList()
        };
    }

    public async Task<int> CreateAsync(int employeeId, CreateTimeOffRequestModel model)
    {
        await _createValidator.ValidateAndThrowAsync(model);

        var entity = new LeaveRequestEntity
        {
            EmployeeId = employeeId,
            Type = model.Type,
            StartDateUtc = DateTime.SpecifyKind(model.StartDate.Date, DateTimeKind.Utc),
            EndDateUtc = DateTime.SpecifyKind(model.EndDate.Date, DateTimeKind.Utc),
            Note = model.Note ?? string.Empty,
            Status = LeaveRequestStatus.Pending
        };

        await _leaveRequestRepository.AddAsync(entity);
        return entity.Id;
    }

    public async Task WithdrawAsync(int employeeId, int id)
    {
        var request = await _leaveRequestRepository.GetByIdAsync(id);

        if (request is null || request.EmployeeId != employeeId)
        {
            throw new NotFoundException($"Time off request with id {id} was not found.");
        }

        if (request.Status != LeaveRequestStatus.Pending)
        {
            throw new InfigoHR.Application.Exceptions.ValidationException("Only pending requests can be withdrawn.");
        }

        request.Status = LeaveRequestStatus.Withdrawn;
        await _leaveRequestRepository.UpdateAsync(request);
    }

    private async Task<EmployeeEntity> GetEmployeeAsync(int employeeId) =>
        await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee with id {employeeId} was not found.");

    private async Task<string> GetApproverNameAsync(EmployeeEntity employee)
    {
        if (employee.ManagerId is null) return "—";
        var manager = await _employeeRepository.GetByIdAsync(employee.ManagerId.Value);
        return manager is null ? "—" : $"{manager.FirstName} {manager.LastName}";
    }

    private static TimeOffBalanceModel BuildVacationBalance(List<LeaveRequestEntity> yearRequests)
    {
        var vacation = yearRequests.Where(r => r.Type == LeaveType.Vacation).ToList();
        var used = vacation.Where(r => r.Status == LeaveRequestStatus.Approved)
            .Sum(r => TimeOffCalculator.WorkingDays(r.StartDateUtc, r.EndDateUtc));
        var pending = vacation.Where(r => r.Status == LeaveRequestStatus.Pending)
            .Sum(r => TimeOffCalculator.WorkingDays(r.StartDateUtc, r.EndDateUtc));
        var left = AnnualVacationDays - used - pending;

        return new TimeOffBalanceModel
        {
            Type = LeaveType.Vacation,
            Label = "Vacation",
            Headline = new TimeOffHeadlineModel { Value = left, Label = "days left" },
            Used = used,
            Pending = pending,
            Total = AnnualVacationDays,
            Caption = $"{used} used · {pending} requested · {left} left"
        };
    }

    private static TimeOffBalanceModel BuildSickBalance(List<LeaveRequestEntity> yearRequests)
    {
        var used = yearRequests.Where(r => r.Type == LeaveType.Sick && r.Status == LeaveRequestStatus.Approved)
            .Sum(r => TimeOffCalculator.WorkingDays(r.StartDateUtc, r.EndDateUtc));

        return new TimeOffBalanceModel
        {
            Type = LeaveType.Sick,
            Label = "Sick",
            Headline = new TimeOffHeadlineModel { Value = used, Label = "days taken" },
            Used = used,
            Pending = 0,
            Total = null,
            Caption = "No limit"
        };
    }

    private static TimeOffBalanceModel BuildRemoteBalance(List<LeaveRequestEntity> monthRequests)
    {
        var used = monthRequests.Where(r => r.Type == LeaveType.Remote && r.Status == LeaveRequestStatus.Approved)
            .Sum(r => TimeOffCalculator.WorkingDays(r.StartDateUtc, r.EndDateUtc));

        return new TimeOffBalanceModel
        {
            Type = LeaveType.Remote,
            Label = "Remote",
            Headline = new TimeOffHeadlineModel { Value = used, Label = "days this month" },
            Used = used,
            Pending = 0,
            Total = null,
            Caption = "Hybrid schedule"
        };
    }
}