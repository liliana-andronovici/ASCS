using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Interfaces;

public interface ITwoFactorService
{
    Task<TwoFactorStatusResponse> GetStatusAsync(
        int employeeId,
        Guid tokenVersion,
        CancellationToken cancellationToken = default);

    Task<TwoFactorSetupResponse> SetupAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorSetupRequest request,
        CancellationToken cancellationToken = default);

    Task<TwoFactorRecoveryResponse> EnableAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorEnableRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthResponse> VerifyAsync(
        TwoFactorChallengeRequest request,
        CancellationToken cancellationToken = default);

    Task<TwoFactorRecoveryResponse> RegenerateAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthResponse> DisableAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken = default);
}
