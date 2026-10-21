namespace InfigoHR.Application.Interfaces.Services;

public interface ICurrentUserAccessor
{
    int EmployeeId { get; }
    IReadOnlyCollection<string> Roles { get; }
    bool IsInRole(string role);
}
