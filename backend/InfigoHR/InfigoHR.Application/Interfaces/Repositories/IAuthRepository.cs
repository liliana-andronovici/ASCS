using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IAuthRepository : IGenericRepository<AuthEntity>
{
    Task<bool> TryUpdateAuthenticationAsync(
        AuthEntity auth,
        Guid expectedConcurrencyVersion,
        CancellationToken cancellationToken = default);

    Task<bool> IsSessionValidAsync(
        int employeeId,
        Guid tokenVersion,
        CancellationToken cancellationToken = default);
}
