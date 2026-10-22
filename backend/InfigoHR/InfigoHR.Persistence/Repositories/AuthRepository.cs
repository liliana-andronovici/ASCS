using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using InfigoHR.Persistence.Updates;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class AuthRepository : GenericRepository<AuthEntity>, IAuthRepository
{
    public AuthRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> TryUpdateAuthenticationAsync(
        AuthEntity auth,
        Guid expectedConcurrencyVersion,
        CancellationToken cancellationToken = default)
    {
        var affectedRows = await base.UpdateAsync(
            current =>
                current.Id == auth.Id &&
                current.ConcurrencyVersion == expectedConcurrencyVersion &&
                current.Employee!.Status == EmployeeStatus.Active,
            setters => AuthUpdateSetters.Apply(setters, auth),
            cancellationToken);

        return affectedRows == 1;
    }

    public Task<bool> IsSessionValidAsync(
        int employeeId,
        Guid tokenVersion,
        CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(
            auth =>
                auth.EmployeeId == employeeId &&
                auth.Employee!.Status == EmployeeStatus.Active &&
                auth.TokenVersion == tokenVersion,
            cancellationToken);
    }
}
