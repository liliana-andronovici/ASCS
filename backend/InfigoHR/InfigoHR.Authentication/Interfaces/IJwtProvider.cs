namespace InfigoHR.Authentication.Interfaces;

public interface IJwtProvider
{
    string Generate(
        int employeeId,
        string email,
        IEnumerable<string> permissions,
        string role);
}