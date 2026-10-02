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
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<AddOnboardingTaskModel> _addTaskValidator;

    public OnboardingService(
        IOnboardingRepository onboardingRepository,
        IEmployeeRepository employeeRepository,
        IValidator<AddOnboardingTaskModel> addTaskValidator)
    {
        _onboardingRepository = onboardingRepository;
        _employeeRepository = employeeRepository;
        _addTaskValidator = addTaskValidator;
    }

    public async Task<OnboardingChecklistModel> GetChecklistAsync(int employeeId)
    {
        var onboarding = await GetOrCreateOnboardingAsync(employeeId);
        return OnboardingMapper.ToChecklistModel(onboarding);
    }

    public async Task AddTaskAsync(int employeeId, AddOnboardingTaskModel model)
    {
        await _addTaskValidator.ValidateAndThrowAsync(model);

        var onboarding = await GetOrCreateOnboardingAsync(employeeId);

        var task = new OnboardingTaskEntity
        {
            OnboardingId = onboarding.Id,
            Section = OnboardingMapper.ParseSectionKey(model.SectionKey),
            Title = model.Title,
            Note = model.Note,
            DueDateUtc = ParseDueDate(model.DueDate)
        };

        await _onboardingRepository.AddTaskAsync(task);
    }

    public async Task ToggleTaskAsync(int employeeId, int taskId)
    {
        var task = await _onboardingRepository.GetTaskByIdAsync(taskId)
            ?? throw new NotFoundException($"Task with id {taskId} was not found.");

        if (task.Onboarding is null || task.Onboarding.EmployeeId != employeeId)
        {
            throw new NotFoundException($"Task with id {taskId} was not found.");
        }

        task.CompletedOnUtc = task.CompletedOnUtc is null ? DateTime.UtcNow : null;
        await _onboardingRepository.UpdateTaskAsync(task);
    }

    private async Task<OnboardingEntity> GetOrCreateOnboardingAsync(int employeeId)
    {
        var onboarding = await _onboardingRepository.GetByEmployeeIdAsync(employeeId);
        if (onboarding is not null) return onboarding;

        _ = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee with id {employeeId} was not found.");

        var created = new OnboardingEntity { EmployeeId = employeeId };
        await _onboardingRepository.AddAsync(created);

        return await _onboardingRepository.GetByEmployeeIdAsync(employeeId)
            ?? throw new NotFoundException("Failed to create onboarding checklist.");
    }

    private static DateTime? ParseDueDate(string? dueDate) =>
        !string.IsNullOrWhiteSpace(dueDate) && DateTime.TryParse(dueDate, out var parsed)
            ? DateTime.SpecifyKind(parsed.Date, DateTimeKind.Utc)
            : null;
}