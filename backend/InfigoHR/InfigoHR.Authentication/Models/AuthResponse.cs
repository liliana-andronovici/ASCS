namespace InfigoHR.Authentication.Models;

public class AuthResponse
{
    public string? AccessToken { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string? ChallengeToken { get; set; }
    public DateTime? ChallengeExpiresAtUtc { get; set; }
}
