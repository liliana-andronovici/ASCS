namespace EmailService;

/// <summary>
/// Minimal in-memory store for development and simple testing.
/// Data is lost when the process stops.
/// </summary>
public sealed class InMemoryEmailServiceStore : IEmailServiceStore
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, EmailServiceRecord> _records = new();
    private readonly Dictionary<string, Guid> _tokenIndex = new(StringComparer.Ordinal);

    public Task AddAsync(
        EmailServiceRecord record,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_records.ContainsKey(record.Id) || _tokenIndex.ContainsKey(record.TokenHash))
            {
                throw new InvalidOperationException("The generated link already exists.");
            }

            _records.Add(record.Id, record);
            _tokenIndex.Add(record.TokenHash, record.Id);
        }

        return Task.CompletedTask;
    }

    public Task<EmailServiceRecord?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_tokenIndex.TryGetValue(tokenHash, out var id) &&
                _records.TryGetValue(id, out var record))
            {
                return Task.FromResult<EmailServiceRecord?>(Copy(record));
            }
        }

        return Task.FromResult<EmailServiceRecord?>(null);
    }

    public Task<bool> TryMarkUsedAsync(
        string tokenHash,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (!_tokenIndex.TryGetValue(tokenHash, out var id) ||
                !_records.TryGetValue(id, out var record) ||
                record.UsedAtUtc is not null ||
                record.RevokedAtUtc is not null ||
                record.ExpiresAtUtc <= usedAtUtc)
            {
                return Task.FromResult(false);
            }

            record.UsedAtUtc = usedAtUtc;
            return Task.FromResult(true);
        }
    }

    public Task<bool> TryRevokeAsync(
        Guid id,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (!_records.TryGetValue(id, out var record))
            {
                return Task.FromResult(false);
            }

            record.RevokedAtUtc ??= revokedAtUtc;
            return Task.FromResult(true);
        }
    }

    private static EmailServiceRecord Copy(EmailServiceRecord record) => new()
    {
        Id = record.Id,
        TokenHash = record.TokenHash,
        Data = record.Data,
        CreatedAtUtc = record.CreatedAtUtc,
        ExpiresAtUtc = record.ExpiresAtUtc,
        UsedAtUtc = record.UsedAtUtc,
        RevokedAtUtc = record.RevokedAtUtc
    };
}
