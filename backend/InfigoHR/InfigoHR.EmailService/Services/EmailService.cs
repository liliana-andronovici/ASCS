using System.Security.Cryptography;
using System.Text;

namespace EmailService;

public sealed class EmailService
{
    private static readonly TimeSpan DefaultLifetime = TimeSpan.FromHours(24);
    private readonly IEmailServiceStore _store;
    private readonly TimeProvider _timeProvider;

    public EmailService(
        IEmailServiceStore store,
        TimeProvider? timeProvider = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<EmailServiceCreated> CreateAsync(
        string baseUrl,
        string? data = null,
        TimeSpan? lifetime = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("Base URL is required.", nameof(baseUrl));
        }

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Base URL must be an absolute URL.", nameof(baseUrl));
        }

        var effectiveLifetime = lifetime ?? DefaultLifetime;
        if (effectiveLifetime <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lifetime),
                "Lifetime must be greater than zero.");
        }

        var now = _timeProvider.GetUtcNow();
        var token = GenerateToken();
        var tokenHash = HashToken(token);

        var record = new EmailServiceRecord
        {
            Id = Guid.NewGuid(),
            TokenHash = tokenHash,
            Data = data,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.Add(effectiveLifetime)
        };

        await _store.AddAsync(record, cancellationToken);

        return new EmailServiceCreated(
            record.Id,
            token,
            AddTokenToUrl(baseUrl, token),
            record.ExpiresAtUtc);
    }

    public async Task<EmailServiceValidationResult> ValidateAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var record = await FindAsync(token, cancellationToken);
        return record is null
            ? new EmailServiceValidationResult(EmailServiceState.NotFound)
            : ToValidationResult(record, _timeProvider.GetUtcNow());
    }

    public async Task<EmailServiceValidationResult> ConsumeAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new EmailServiceValidationResult(EmailServiceState.NotFound);
        }

        var tokenHash = HashToken(token);
        var now = _timeProvider.GetUtcNow();
        var consumed = await _store.TryMarkUsedAsync(tokenHash, now, cancellationToken);

        var record = await _store.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (record is null)
        {
            return new EmailServiceValidationResult(EmailServiceState.NotFound);
        }

        if (consumed)
        {
            return new EmailServiceValidationResult(
                EmailServiceState.Used,
                record.Id,
                record.Data,
                record.ExpiresAtUtc);
        }

        return ToValidationResult(record, now);
    }

    public Task<bool> RevokeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _store.TryRevokeAsync(
            id,
            _timeProvider.GetUtcNow(),
            cancellationToken);
    }

    private async Task<EmailServiceRecord?> FindAsync(
        string token,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return await _store.GetByTokenHashAsync(HashToken(token), cancellationToken);
    }

    private static EmailServiceValidationResult ToValidationResult(
        EmailServiceRecord record,
        DateTimeOffset now)
    {
        var state = record.RevokedAtUtc is not null
            ? EmailServiceState.Revoked
            : record.UsedAtUtc is not null
                ? EmailServiceState.Used
                : record.ExpiresAtUtc <= now
                    ? EmailServiceState.Expired
                    : EmailServiceState.Valid;

        return new EmailServiceValidationResult(
            state,
            record.Id,
            record.Data,
            record.ExpiresAtUtc);
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }

    private static string AddTokenToUrl(string baseUrl, string token)
    {
        var fragmentIndex = baseUrl.IndexOf('#');
        var fragment = fragmentIndex >= 0 ? baseUrl[fragmentIndex..] : string.Empty;
        var withoutFragment = fragmentIndex >= 0 ? baseUrl[..fragmentIndex] : baseUrl;
        var separator = withoutFragment.Contains('?') ? '&' : '?';

        return $"{withoutFragment}{separator}token={Uri.EscapeDataString(token)}{fragment}";
    }
}
