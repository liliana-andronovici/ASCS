using InfigoHR.Authentication.Models;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Authentication.Mappers;

public static class AuthMapper
{
    public static EmployeeEntity ToEntity(SignUpRequest request)
    {
        return new EmployeeEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Title = request.Title,
            StartedOnUtc = request.StartedOn!.Value.UtcDateTime,
            Status = EmployeeStatus.Active
        };
    }
}