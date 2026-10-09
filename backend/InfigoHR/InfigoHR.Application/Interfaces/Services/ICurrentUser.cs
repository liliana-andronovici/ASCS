namespace InfigoHR.Application.Interfaces.Services;

public interface ICurrentUser
{
    int EmployeeId { get; }
    string Role { get; }
}