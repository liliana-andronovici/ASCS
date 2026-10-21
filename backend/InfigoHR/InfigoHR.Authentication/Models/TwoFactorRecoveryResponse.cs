namespace InfigoHR.Authentication.Models;

public class TwoFactorRecoveryResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string[] RecoveryCodes { get; set; } = [];
}
