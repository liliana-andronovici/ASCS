using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class AuthService(AppDbContext dbContext) : IAuthService
{
    public async Task<IReadOnlyList<AuthDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Auths.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new AuthDto(
                x.Id,
                x.EmployeeId,
                !string.IsNullOrEmpty(x.PasswordHash),
                x.ResetTokenExpiresAtUtc))
            .ToListAsync(cancellationToken);

    public async Task<AuthDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Auths.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AuthDto(
                x.Id,
                x.EmployeeId,
                !string.IsNullOrEmpty(x.PasswordHash),
                x.ResetTokenExpiresAtUtc))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<AuthDto> CreateAsync(CreateAuthDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Auth
        {
            EmployeeId = dto.EmployeeId,
            PasswordHash = dto.PasswordHash,
            ResetToken = dto.ResetToken,
            ResetTokenExpiresAtUtc = dto.ResetTokenExpiresAtUtc
        };
        dbContext.Auths.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AuthDto(
            entity.Id,
            entity.EmployeeId,
            !string.IsNullOrEmpty(entity.PasswordHash),
            entity.ResetTokenExpiresAtUtc);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAuthDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Auths.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.PasswordHash = dto.PasswordHash;
        entity.ResetToken = dto.ResetToken;
        entity.ResetTokenExpiresAtUtc = dto.ResetTokenExpiresAtUtc;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Auths.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        dbContext.Auths.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
