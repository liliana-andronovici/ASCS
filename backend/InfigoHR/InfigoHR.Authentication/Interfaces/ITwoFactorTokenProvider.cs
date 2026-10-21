namespace InfigoHR.Authentication.Interfaces;

public interface ITwoFactorTokenProvider
{
    string GenerateSecret();
    string ProtectSecret(int employeeId, string secret);
    bool TryVerifyCode(int employeeId, string protectedSecret, string code, DateTime now, out long timeStep);
    string GetAuthenticatorUri(string email, string secret);
    string GenerateChallenge();
    string HashChallenge(string token);
    string[] GenerateRecoveryCodes();
    string HashRecoveryCode(int employeeId, string code);
}
