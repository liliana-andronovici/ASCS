namespace InfigoHR.Authentication.Models;

public class TwoFactorChallengeRequest
{
    public string ChallengeToken { get; set; } = string.Empty;

    public string? Code { get; set; }
    public string? RecoveryCode { get; set; }
}
