using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.LeaveRequests;
using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly IEmployeeRepository _employees;
    private readonly IValidator<CreateLeaveRequestModel> _createValidator;
    private readonly IValidator<UpdateLeaveRequestModel> _updateValidator;

    public LeaveRequestService(
        ILeaveRequestRepository leaveRequestRepository,
        IEmployeeRepository employees,
        ICurrentUserAccessor currentUserAccessor,
        IValidator<CreateLeaveRequestModel> createValidator,
        IValidator<UpdateLeaveRequestModel> updateValidator)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _employees = employees;
        _currentUserAccessor = currentUserAccessor;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public Task<int> GetOutTodayCountAsync(DateOnly date) =>
        _leaveRequestRepository.CountApprovedOnAsync(date);

    public async Task<TimeOffOverviewModel> GetOverviewAsync()
    {
        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(_currentUserAccessor.EmployeeId);
        var year = DateTime.UtcNow.Year;
        var balances = new List<TimeOffBalanceModel>();
        foreach (var type in Enum.GetValues<LeaveType>())
        {
            var matching = requests.Where(request => request.Type == type && request.StartDateUtc.Year == year).ToList();
            var used = matching.Where(request => request.Status == LeaveRequestStatus.Approved)
                .Sum(request => CountWorkingDays(DateOnly.FromDateTime(request.StartDateUtc), DateOnly.FromDateTime(request.EndDateUtc)));
            var pending = matching.Where(request => request.Status == LeaveRequestStatus.Pending)
                .Sum(request => CountWorkingDays(DateOnly.FromDateTime(request.StartDateUtc), DateOnly.FromDateTime(request.EndDateUtc)));
            balances.Add(new TimeOffBalanceModel
            {
                Type = type,
                Label = type.ToString(),
                Headline = new TimeOffHeadlineModel { Value = used + pending, Label = "days requested" },
                Used = used,
                Pending = pending,
                Caption = $"{used} approved · {pending} pending"
            });
        }

        return new TimeOffOverviewModel
        {
            ApproverName = await GetApproverNameAsync(),
            Balances = balances
        };
    }

    public async Task<IReadOnlyList<TimeOffRequestSummaryModel>> GetRequestSummariesAsync()
    {
        var approverName = await GetApproverNameAsync();
        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(_currentUserAccessor.EmployeeId);
        return requests.Select(request => new TimeOffRequestSummaryModel
        {
            Id = request.Id,
            Type = request.Type,
            StartDate = DateOnly.FromDateTime(request.StartDateUtc),
            EndDate = DateOnly.FromDateTime(request.EndDateUtc),
            Days = CountWorkingDays(DateOnly.FromDateTime(request.StartDateUtc), DateOnly.FromDateTime(request.EndDateUtc)),
            ApproverName = approverName,
            Status = request.Status,
            CanWithdraw = request.Status == LeaveRequestStatus.Pending
        }).ToList();
    }

    public async Task<TimeOffPreviewModel> PreviewAsync(TimeOffPreviewInputModel model)
    {
        if (!Enum.IsDefined(model.Type) || model.StartDate == default || model.EndDate == default ||
            model.EndDate < model.StartDate || model.StartDate.Year != model.EndDate.Year ||
            model.EndDate.DayNumber - model.StartDate.DayNumber > 365)
            throw new Exceptions.ValidationException("Select a valid time-off type and date range of up to one year.");

        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(_currentUserAccessor.EmployeeId);
        if (requests.Any(request => (request.Status is LeaveRequestStatus.Pending or LeaveRequestStatus.Approved) &&
                                    DateOnly.FromDateTime(request.StartDateUtc) <= model.EndDate &&
                                    DateOnly.FromDateTime(request.EndDateUtc) >= model.StartDate))
            throw new Exceptions.ValidationException("You already have a request for these dates.");

        var workingDays = CountWorkingDays(model.StartDate, model.EndDate);
        if (workingDays == 0)
            throw new Exceptions.ValidationException("Select at least one working day.");
        return new TimeOffPreviewModel { WorkingDays = workingDays };
    }

    public async Task<int> CreateFromPageAsync(TimeOffRequestInputModel model)
    {
        await PreviewAsync(model);
        return await CreateAsync(new CreateLeaveRequestModel
        {
            Type = model.Type,
            StartDateUtc = model.StartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            EndDateUtc = model.EndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            Note = model.Note ?? string.Empty
        });
    }

    public async Task WithdrawAsync(int id)
    {
        var request = await GetOwnedRequestAsync(id);
        EnsurePending(request);
        request.Status = LeaveRequestStatus.Withdrawn;
        await _leaveRequestRepository.UpdateAsync(request);
    }

    private async Task<string> GetApproverNameAsync()
    {
        var employee = await _employees.GetByIdAsync(_currentUserAccessor.EmployeeId);
        if (employee?.ManagerId is not int managerId) return "Your manager";
        var manager = await _employees.GetByIdAsync(managerId);
        return manager is null ? "Your manager" : $"{manager.FirstName} {manager.LastName}";
    }

    private static int CountWorkingDays(DateOnly start, DateOnly end)
    {
        var count = 0;
        for (var day = start; day <= end; day = day.AddDays(1))
            if (day.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)) count++;
        return count;
    }

    public async Task<IReadOnlyList<LeaveRequestModel>> GetAllAsync()
    {
        var employeeId = _currentUserAccessor.EmployeeId;
        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId);

        return requests.Select(LeaveRequestMapper.ToModel).ToList();
    }

    public async Task<LeaveRequestModel> GetByIdAsync(int id)
    {
        var entity = await GetOwnedRequestAsync(id);
        return LeaveRequestMapper.ToModel(entity);
    }

    public async Task<int> CreateAsync(CreateLeaveRequestModel model)
    {
        var employeeId = _currentUserAccessor.EmployeeId;
        await _createValidator.ValidateAndThrowAsync(model);

        var entity = LeaveRequestMapper.ToEntity(model, employeeId);
        await _leaveRequestRepository.AddAsync(entity);

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateLeaveRequestModel model)
    {
        await _updateValidator.ValidateAndThrowAsync(model);
        var entity = await GetOwnedRequestAsync(model.Id);
        EnsurePending(entity);

        LeaveRequestMapper.ToEntity(model, entity);
        await _leaveRequestRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetOwnedRequestAsync(id);
        EnsurePending(entity);

        await _leaveRequestRepository.DeleteAsync(entity);
    }

    private async Task<LeaveRequestEntity> GetOwnedRequestAsync(int id)
    {
        var employeeId = _currentUserAccessor.EmployeeId;

        var entity = await _leaveRequestRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Leave request with id {id} was not found.");

        if (entity.EmployeeId != employeeId)
        {
            throw new ForbiddenException("You cannot access another employee's leave request.");
        }

        return entity;
    }

    private static void EnsurePending(LeaveRequestEntity entity)
    {
        if (entity.Status != LeaveRequestStatus.Pending)
        {
            throw new Exceptions.ValidationException("Only pending leave requests can be changed or deleted.");
        }
    }
}
