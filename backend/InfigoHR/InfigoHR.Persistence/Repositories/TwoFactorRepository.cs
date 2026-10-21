using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class TwoFactorRepository : ITwoFactorRepository
{
    private readonly AppDbContext _dbContext;

    public TwoFactorRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private IQueryable<EmployeeEntity> Query =>
        _dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Auth)
            .Include(employee => employee.TwoFactorAuth)
            .Include(employee => employee.Roles)
            .ThenInclude(role => role.Permissions);

    public Task<EmployeeEntity?> GetByIdAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        return Query.SingleOrDefaultAsync(
            employee => employee.Id == employeeId,
            cancellationToken);
    }

    public Task<EmployeeEntity?> GetByChallengeHashAsync(
        string challengeHash,
        CancellationToken cancellationToken = default)
    {
        return Query.SingleOrDefaultAsync(
            employee =>
                employee.TwoFactorAuth != null &&
                employee.TwoFactorAuth.ChallengeTokenHash == challengeHash,
            cancellationToken);
    }

    public async Task<bool> TrySaveAsync(
        EmployeeEntity employee,
        Guid expectedConcurrencyVersion,
        Guid newTokenVersion,
        DateTime? validUntilUtc,
        CancellationToken cancellationToken = default)
    {
        var auth = employee.Auth
            ?? throw new InvalidOperationException(
                "Authentication state is required.");

        var nextConcurrencyVersion = Guid.NewGuid();

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var affectedRows = await _dbContext.Auths
            .Where(current =>
                current.Id == auth.Id &&
                current.EmployeeId == employee.Id &&
                current.ConcurrencyVersion == expectedConcurrencyVersion &&
                current.Employee!.Status == EmployeeStatus.Active)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        current => current.ConcurrencyVersion,
                        nextConcurrencyVersion)
                    .SetProperty(
                        current => current.TokenVersion,
                        newTokenVersion)
                    .SetProperty(
                        current => current.UpdatedOnUtc,
                        DateTime.UtcNow),
                cancellationToken);

        if (affectedRows != 1)
        {
            return false;
        }

        var credentialExpired =
            validUntilUtc.HasValue &&
            validUntilUtc.Value <= DateTime.UtcNow;

        if (credentialExpired)
        {
            return false;
        }

        var state = employee.TwoFactorAuth;

        if (state is not null)
        {
            state.EmployeeId = employee.Id;
            state.Employee = null;
            state.UpdatedOnUtc = DateTime.UtcNow;

            _dbContext.Entry(state).State = state.Id == 0
                ? EntityState.Added
                : EntityState.Modified;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        auth.ConcurrencyVersion = nextConcurrencyVersion;
        auth.TokenVersion = newTokenVersion;

        if (state is not null)
        {
            _dbContext.Entry(state).State = EntityState.Detached;
        }

        return true;
    }
}