namespace EmailService;

public sealed record EmailServiceCreated(
    Guid Id,
    string Url,
    DateTimeOffset ExpiresAtUtc);

public enum EmailServiceState
{
    Valid,
    Expired,
    Used,
    Revoked,
    NotFound
}

public sealed record EmailServiceValidationResult(
    EmailServiceState State,
    Guid? Id = null,
    string? Data = null,
    DateTimeOffset? ExpiresAtUtc = null)
{
    public bool IsValid => State == EmailServiceState.Valid;
}
