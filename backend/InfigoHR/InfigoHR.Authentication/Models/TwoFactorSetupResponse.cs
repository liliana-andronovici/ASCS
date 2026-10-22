namespace InfigoHR.Authentication.Models;

public class TwoFactorSetupResponse
{
    public Guid SetupId { get; set; }

    public string SharedKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}
