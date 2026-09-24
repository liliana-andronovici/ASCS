namespace EmailService;

public interface IEmailServiceStore
{
    Task AddAsync(
        EmailServiceRecord record,
        CancellationToken cancellationToken = default);

    Task<EmailServiceRecord?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<bool> TryMarkUsedAsync(
        string tokenHash,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken = default);

    Task<bool> TryRevokeAsync(
        Guid id,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken = default);
}
