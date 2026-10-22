namespace InfigoHR.Authentication.Interfaces;

public interface IJwtProvider
{
    string Generate(
        int employeeId,
        string email,
        Guid tokenVersion,
        IEnumerable<string> permissions,
        IEnumerable<string> roles);
}