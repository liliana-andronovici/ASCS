using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace InfigoHR.Persistence.Services;
public sealed class TwoFactorAuthService(AppDbContext dbContext) : ITwoFactorAuthService
{
    public async Task<IReadOnlyList<TwoFactorAuthDto>> GetAllAsync(CancellationToken cancellationToken = default) => await dbContext.TwoFactorAuths.AsNoTracking().OrderBy(x => x.EmployeeId).Select(x => new TwoFactorAuthDto(x.EmployeeId, x.SecretKey != string.Empty)).ToListAsync(cancellationToken);
    public async Task<TwoFactorAuthDto?> GetByIdAsync(int employeeId, CancellationToken cancellationToken = default) => await dbContext.TwoFactorAuths.AsNoTracking().Where(x => x.EmployeeId == employeeId).Select(x => new TwoFactorAuthDto(x.EmployeeId, x.SecretKey != string.Empty)).SingleOrDefaultAsync(cancellationToken);
    public async Task<TwoFactorAuthDto> CreateAsync(CreateTwoFactorAuthDto dto, CancellationToken cancellationToken = default) { var e = new TwoFactorAuth { EmployeeId = dto.EmployeeId, SecretKey = dto.SecretKey }; dbContext.TwoFactorAuths.Add(e); await dbContext.SaveChangesAsync(cancellationToken); return new TwoFactorAuthDto(e.EmployeeId, e.SecretKey != string.Empty); }
    public async Task<bool> UpdateAsync(int employeeId, UpdateTwoFactorAuthDto dto, CancellationToken cancellationToken = default) { var e = await dbContext.TwoFactorAuths.SingleOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken); if (e is null) return false; e.SecretKey = dto.SecretKey; await dbContext.SaveChangesAsync(cancellationToken); return true; }
    public async Task<bool> DeleteAsync(int employeeId, CancellationToken cancellationToken = default) { var e = await dbContext.TwoFactorAuths.SingleOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken); if (e is null) return false; dbContext.TwoFactorAuths.Remove(e); await dbContext.SaveChangesAsync(cancellationToken); return true; }
}
