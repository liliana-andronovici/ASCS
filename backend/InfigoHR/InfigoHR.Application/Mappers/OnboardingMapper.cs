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

    public static string ToSectionKey(OnboardingSection section) => section switch
    {
        OnboardingSection.BeforeDayOne => "beforeDayOne",
        OnboardingSection.FirstWeek => "firstWeek",
        OnboardingSection.FirstMonth => "firstMonth",
        OnboardingSection.Other => "other",
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

    public static OnboardingSection ParseSectionKey(string key) => key switch
    {
        "beforeDayOne" => OnboardingSection.BeforeDayOne,
        "firstWeek" => OnboardingSection.FirstWeek,
        "firstMonth" => OnboardingSection.FirstMonth,
        "other" => OnboardingSection.Other,
        _ => throw new ValidationException($"'{key}' is not a valid onboarding section.")
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
    
    public static OnboardingSummaryModel ToSummaryModel(OnboardingEntity onboarding)
    {
        var employee = onboarding.Employee!;
        return new OnboardingSummaryModel
        {
            EmployeeId = onboarding.EmployeeId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Title = employee.Title,
            Department = employee.Department?.Name ?? "—",
            StartsOn = employee.StartedOnUtc,
            CompletedTaskCount = onboarding.Tasks.Count(t => t.CompletedOnUtc != null),
            TotalTaskCount = onboarding.Tasks.Count,
            IsComplete = onboarding.Tasks.Count > 0 && onboarding.Tasks.All(t => t.CompletedOnUtc != null)
        };
    }
}