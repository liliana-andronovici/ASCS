using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class TwoFactorAuthService(AppDbContext dbContext) : ITwoFactorAuthService
{
    public async Task<IReadOnlyList<TwoFactorAuthDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.TwoFactorAuths.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new TwoFactorAuthDto(x.Id, x.EmployeeId, !string.IsNullOrEmpty(x.SecretKey)))
            .ToListAsync(cancellationToken);

    public async Task<TwoFactorAuthDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.TwoFactorAuths.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TwoFactorAuthDto(x.Id, x.EmployeeId, !string.IsNullOrEmpty(x.SecretKey)))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<TwoFactorAuthDto> CreateAsync(
        CreateTwoFactorAuthDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new TwoFactorAuth { EmployeeId = dto.EmployeeId, SecretKey = dto.SecretKey };
        dbContext.TwoFactorAuths.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new TwoFactorAuthDto(entity.Id, entity.EmployeeId, !string.IsNullOrEmpty(entity.SecretKey));
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateTwoFactorAuthDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.TwoFactorAuths.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.SecretKey = dto.SecretKey;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.TwoFactorAuths.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        dbContext.TwoFactorAuths.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
