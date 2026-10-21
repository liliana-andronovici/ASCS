namespace InfigoHR.Authentication.Options;

public class TwoFactorOptions
{
    public string Issuer { get; set; } = string.Empty;

    public int SetupExpirationMinutes { get; set; }
    public int ChallengeExpirationMinutes { get; set; }

    public int MaximumAttempts { get; set; }
    public int LockoutMinutes { get; set; }

    public int RecoveryCodeCount { get; set; }
    public int RequestsPerMinute { get; set; }
}