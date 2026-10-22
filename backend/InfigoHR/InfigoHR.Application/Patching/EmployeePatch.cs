using System.Text.Json;
using System.Text.Json.Serialization;
using InfigoHR.Application.Exceptions;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Patching;

public static class EmployeePatch
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };

    public static long GetVersion(JsonElement body)
    {
        if (body.ValueKind != JsonValueKind.Object ||
            !body.TryGetProperty("profileVersion", out var version) ||
            version.ValueKind != JsonValueKind.Number ||
            !version.TryGetInt64(out var value) || value < 0)
        {
            throw new ValidationException("A nonnegative profileVersion is required.");
        }

        return value;
    }

    public static void Apply(JsonElement body, EmployeeEntity employee)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in body.EnumerateObject())
        {
            if (!seen.Add(property.Name))
            {
                throw new ValidationException($"Duplicate field: {property.Name}.");
            }

            switch (property.Name)
            {
                case "profileVersion": break;
                case "firstName": employee.FirstName = Read<string>(property); break;
                case "lastName": employee.LastName = Read<string>(property); break;
                case "email": employee.Email = Read<string>(property); break;
                case "title": employee.Title = Read<string>(property); break;
                case "startedOnUtc": employee.StartedOnUtc = Read<DateTime>(property); break;
                case "departmentId": employee.DepartmentId = Read<int?>(property); break;
                case "locationId": employee.LocationId = Read<int?>(property); break;
                case "managerId": employee.ManagerId = Read<int?>(property); break;
                case "status": employee.Status = Read<EmployeeStatus>(property); break;
                case "phone": employee.Phone = Read<string?>(property); break;
                case "desk": employee.Desk = Read<string?>(property); break;
                case "birthdayMonth": employee.BirthdayMonth = Read<int?>(property); break;
                case "birthdayDay": employee.BirthdayDay = Read<int?>(property); break;
                case "languages": employee.Languages = Read<string[]>(property); break;
                case "employeeCode": employee.EmployeeCode = Read<string?>(property); break;
                case "employmentType": employee.EmploymentType = Read<string?>(property); break;
                case "contractType": employee.ContractType = Read<string?>(property); break;
                case "weeklyHours": employee.WeeklyHours = Read<decimal?>(property); break;
                default: throw new ValidationException($"Field is not editable: {property.Name}.");
            }
        }
    }

    private static T Read<T>(JsonProperty property)
    {
        try
        {
            return property.Value.Deserialize<T>(SerializerOptions)!;
        }
        catch (JsonException)
        {
            throw new ValidationException($"Invalid value for {property.Name}.");
        }
    }
}