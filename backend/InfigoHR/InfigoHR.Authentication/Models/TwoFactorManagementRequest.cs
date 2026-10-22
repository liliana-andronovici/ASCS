namespace InfigoHR.Authentication.Models;

public class TwoFactorManagementRequest
{
    public string Password { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? RecoveryCode { get; set; }
}
