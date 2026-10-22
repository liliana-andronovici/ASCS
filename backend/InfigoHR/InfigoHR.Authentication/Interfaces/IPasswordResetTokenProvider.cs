namespace InfigoHR.Authentication.Interfaces;

public interface IPasswordResetTokenProvider
{
    string GenerateCode();
    string GenerateToken();

    string HashCode(int employeeId, string code);
    string HashToken(string token);

    bool Matches(string expectedHash, string actualHash);
}