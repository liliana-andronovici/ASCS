namespace InfigoHR.Authentication.Models;

public class TwoFactorStatusResponse
{
    public bool IsEnabled { get; set; }

    public int RecoveryCodesRemaining { get; set; }
}
