using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Interfaces;

public interface IPasswordResetService
{
    Task RequestAsync(
        RequestPasswordResetRequest request,
        CancellationToken cancellationToken = default);

    Task<VerifyPasswordResetResponse> VerifyAsync(
        VerifyPasswordResetRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthResponse> CompleteAsync(
        CompletePasswordResetRequest request,
        CancellationToken cancellationToken = default);
}