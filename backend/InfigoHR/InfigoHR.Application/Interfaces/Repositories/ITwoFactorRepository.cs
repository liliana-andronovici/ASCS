using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ITwoFactorRepository
{
    Task<EmployeeEntity?> GetByIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<EmployeeEntity?> GetByChallengeHashAsync(
        string challengeHash,
        CancellationToken cancellationToken = default);

    Task<bool> TrySaveAsync(
        EmployeeEntity employee,
        Guid expectedConcurrencyVersion,
        Guid newTokenVersion,
        DateTime? validUntilUtc,
        CancellationToken cancellationToken = default);
}