namespace InfigoHR.Authentication.Models;

public class TwoFactorEnableRequest
{
    public Guid SetupId { get; set; }

    public string Code { get; set; } = string.Empty;
}
