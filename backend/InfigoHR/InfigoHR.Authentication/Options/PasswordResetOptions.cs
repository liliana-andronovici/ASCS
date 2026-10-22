namespace InfigoHR.Authentication.Options;

public class PasswordResetOptions
{
    public int CodeExpirationMinutes { get; set; }
    public int ResetTokenExpirationMinutes { get; set; }
    public int ResendCooldownSeconds { get; set; }
    public int MaxVerificationAttempts { get; set; }
    public int RequestsPerMinute { get; set; }

    public string CodeHashKey { get; set; } = string.Empty;
}