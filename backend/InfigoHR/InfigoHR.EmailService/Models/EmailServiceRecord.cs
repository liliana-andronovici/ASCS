namespace EmailService;

public sealed class EmailServiceRecord
{
    public required Guid Id { get; init; }
    public required string TokenHash { get; init; }
    public string? Data { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
    public required DateTimeOffset ExpiresAtUtc { get; init; }
    public DateTimeOffset? UsedAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
}
