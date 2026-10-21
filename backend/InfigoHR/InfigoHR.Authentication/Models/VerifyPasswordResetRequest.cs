namespace InfigoHR.Authentication.Models;

public class VerifyPasswordResetRequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}