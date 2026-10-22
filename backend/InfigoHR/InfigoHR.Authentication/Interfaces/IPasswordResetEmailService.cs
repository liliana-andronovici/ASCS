namespace InfigoHR.Authentication.Interfaces;

public interface IPasswordResetEmailService
{
    Task SendCodeAsync(
        string recipient,
        string code,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default);

    Task SendPasswordChangedAsync(
        string recipient,
        CancellationToken cancellationToken = default);
}