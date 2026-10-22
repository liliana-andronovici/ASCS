using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public class OnboardingService : IOnboardingService
{
    private readonly IOnboardingRepository _onboardingRepository;
    private readonly IOnboardingTaskRepository _onboardingTaskRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<AddOnboardingTaskModel> _addTaskValidator;

    public OnboardingService(
        IOnboardingRepository onboardingRepository,
        IOnboardingTaskRepository onboardingTaskRepository,
        IEmployeeRepository employeeRepository,
        IValidator<AddOnboardingTaskModel> addTaskValidator)
    {
        _onboardingRepository = onboardingRepository;
        _onboardingTaskRepository = onboardingTaskRepository;
        _employeeRepository = employeeRepository;
        _addTaskValidator = addTaskValidator;
    }

    public async Task<OnboardingChecklistModel> GetChecklistAsync(int employeeId)
    {
        var onboarding = await _onboardingRepository.GetByEmployeeIdWithDetailsAsync(employeeId);
        if (onboarding is null)
        {
            await GetOrCreateOnboardingAsync(employeeId);
            onboarding = await _onboardingRepository.GetByEmployeeIdWithDetailsAsync(employeeId)
                ?? throw new NotFoundException("Failed to create onboarding checklist.");
        }

        return OnboardingMapper.ToChecklistModel(onboarding);
    }

    public async Task AddTaskAsync(int employeeId, AddOnboardingTaskModel model)
    {
        await _addTaskValidator.ValidateAndThrowAsync(model);

        var onboarding = await GetOrCreateOnboardingAsync(employeeId);

        var task = OnboardingMapper.ToTaskEntity(model, onboarding.Id);

        await _onboardingTaskRepository.AddAsync(task);
    }

    public async Task ToggleTaskAsync(int employeeId, int taskId)
    {
        var task = await _onboardingTaskRepository.GetByIdWithOnboardingAsync(taskId)
            ?? throw new NotFoundException($"Task with id {taskId} was not found.");

        if (task.Onboarding is null || task.Onboarding.EmployeeId != employeeId)
        {
            throw new NotFoundException($"Task with id {taskId} was not found.");
        }

        task.CompletedOnUtc = task.CompletedOnUtc is null ? DateTime.UtcNow : null;
        await _onboardingTaskRepository.UpdateAsync(task);
    }

    private async Task<OnboardingEntity> GetOrCreateOnboardingAsync(int employeeId)
    {
        var onboarding = await _onboardingRepository.GetByEmployeeIdAsync(employeeId);
        if (onboarding is not null) return onboarding;

        if (!await _employeeRepository.ExistsAsync(employeeId))
            throw new NotFoundException($"Employee with id {employeeId} was not found.");

        var created = new OnboardingEntity { EmployeeId = employeeId };
        await _onboardingRepository.AddAsync(created);

        return created;
    }

    public async Task<List<OnboardingSummaryModel>> GetOnboardingSummariesAsync()
    {
        var employees = await _employeeRepository.GetAllWithDepartmentAsync();
        var onboardings = await _onboardingRepository.GetAllWithDetailsAsync();
        var onboardingByEmployeeId = onboardings.ToDictionary(o => o.EmployeeId);

        return employees
            .Select(employee =>
            {
                onboardingByEmployeeId.TryGetValue(employee.Id, out var onboarding);
                return OnboardingMapper.ToSummaryModel(employee, onboarding);
            })
            .ToList();
    }

    public async Task<List<UpcomingOnboardingTaskModel>> GetUpcomingTasksAsync(int employeeId)
    {
        var todayUtc = DateTime.UtcNow.Date;
        var tasks = await _onboardingTaskRepository.GetUpcomingForEmployeeAsync(employeeId, todayUtc, todayUtc.AddDays(46));

        return tasks.Select(task => new UpcomingOnboardingTaskModel
        {
            Id = task.Id,
            EmployeeId = employeeId,
            Title = task.Title,
            DueDate = DateOnly.FromDateTime(task.DueDateUtc!.Value)
        }).ToList();
    }
}
