using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Models;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Mappers;

public static class OnboardingMapper
{
    private static readonly OnboardingSection[] SectionOrder =
    {
        OnboardingSection.BeforeDayOne,
        OnboardingSection.FirstWeek,
        OnboardingSection.FirstMonth,
        OnboardingSection.Other
    };

    public static OnboardingSectionKey ToSectionKey(OnboardingSection section) => section switch
    {
        OnboardingSection.BeforeDayOne => OnboardingSectionKey.BeforeDayOne,
        OnboardingSection.FirstWeek => OnboardingSectionKey.FirstWeek,
        OnboardingSection.FirstMonth => OnboardingSectionKey.FirstMonth,
        OnboardingSection.Other => OnboardingSectionKey.Other,
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };

    public static string ToSectionTitle(OnboardingSection section) => section switch
    {
        OnboardingSection.BeforeDayOne => "Before day one",
        OnboardingSection.FirstWeek => "First week",
        OnboardingSection.FirstMonth => "First month",
        OnboardingSection.Other => "Other",
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };

    public static OnboardingSection ParseSectionKey(OnboardingSectionKey key) => key switch
    {
        OnboardingSectionKey.BeforeDayOne => OnboardingSection.BeforeDayOne,
        OnboardingSectionKey.FirstWeek => OnboardingSection.FirstWeek,
        OnboardingSectionKey.FirstMonth => OnboardingSection.FirstMonth,
        OnboardingSectionKey.Other => OnboardingSection.Other,
        _ => throw new ArgumentOutOfRangeException(nameof(key))
    };

    public static OnboardingTaskModel ToTaskModel(OnboardingTaskEntity entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Note = entity.Note,
            DueDate = entity.DueDateUtc,
            CompletedOn = entity.CompletedOnUtc
        };

    public static OnboardingChecklistModel ToChecklistModel(OnboardingEntity onboarding)
    {
        var employee = onboarding.Employee!;

        return new OnboardingChecklistModel
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Title = employee.Title,
            Department = employee.Department?.Name ?? "—",
            Location = employee.Location?.Name ?? "—",
            StartsOn = employee.StartedOnUtc,
            ManagerName = employee.Manager is null ? "—" : $"{employee.Manager.FirstName} {employee.Manager.LastName}",
            BuddyName = onboarding.Buddy is null ? "—" : $"{onboarding.Buddy.FirstName} {onboarding.Buddy.LastName}",
            IsComplete = onboarding.Tasks.Count > 0 && onboarding.Tasks.All(t => t.CompletedOnUtc != null),
            Sections = SectionOrder
                .Select(section => new OnboardingSectionModel
                {
                    Key = ToSectionKey(section),
                    Title = ToSectionTitle(section),
                    Tasks = onboarding.Tasks
                        .Where(t => t.Section == section)
                        .OrderBy(t => t.DueDateUtc)
                        .Select(ToTaskModel)
                        .ToList()
                })
                .ToList()
        };
    }
    public static OnboardingSummaryModel ToSummaryModel(OnboardingEmployeeModel employee, OnboardingEntity? onboarding)
    {
        var tasks = onboarding?.Tasks ?? new List<OnboardingTaskEntity>();
        return new OnboardingSummaryModel
        {
            EmployeeId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Title = employee.Title,
            Department = employee.Department,
            StartsOn = employee.StartedOnUtc,
            CompletedTaskCount = tasks.Count(t => t.CompletedOnUtc != null),
            TotalTaskCount = tasks.Count,
            IsComplete = tasks.Count > 0 && tasks.All(t => t.CompletedOnUtc != null)
        };
    }
    public static OnboardingTaskEntity ToTaskEntity(
        AddOnboardingTaskModel model,
        int onboardingId) =>
        new()
        {
            OnboardingId = onboardingId,
            Section = ParseSectionKey(model.SectionKey!.Value),
            Title = model.Title,
            Note = model.Note,
            DueDateUtc = ParseDueDate(model.DueDate)
        };

    private static DateTime? ParseDueDate(string? dueDate) =>
        !string.IsNullOrWhiteSpace(dueDate) && DateTime.TryParse(dueDate, out var parsed)
            ? DateTime.SpecifyKind(parsed.Date, DateTimeKind.Utc)
            : null;
}
