using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace InfigoHR.Persistence.Services;
public sealed class AuthService(AppDbContext dbContext) : IAuthService
{
    public async Task<IReadOnlyList<AuthDto>> GetAllAsync(CancellationToken cancellationToken = default) => await dbContext.Auths.AsNoTracking().OrderBy(x => x.EmployeeId).Select(x => new AuthDto(x.EmployeeId, x.PasswordHash != string.Empty, x.ResetTokenExpiry)).ToListAsync(cancellationToken);
    public async Task<AuthDto?> GetByIdAsync(int employeeId, CancellationToken cancellationToken = default) => await dbContext.Auths.AsNoTracking().Where(x => x.EmployeeId == employeeId).Select(x => new AuthDto(x.EmployeeId, x.PasswordHash != string.Empty, x.ResetTokenExpiry)).SingleOrDefaultAsync(cancellationToken);
    public async Task<AuthDto> CreateAsync(CreateAuthDto dto, CancellationToken cancellationToken = default) { var e = new Auth { EmployeeId = dto.EmployeeId, PasswordHash = dto.PasswordHash, ResetToken = dto.ResetToken, ResetTokenExpiry = dto.ResetTokenExpiry }; dbContext.Auths.Add(e); await dbContext.SaveChangesAsync(cancellationToken); return new AuthDto(e.EmployeeId, e.PasswordHash != string.Empty, e.ResetTokenExpiry); }
    public async Task<bool> UpdateAsync(int employeeId, UpdateAuthDto dto, CancellationToken cancellationToken = default) { var e = await dbContext.Auths.SingleOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken); if (e is null) return false; e.PasswordHash = dto.PasswordHash; e.ResetToken = dto.ResetToken; e.ResetTokenExpiry = dto.ResetTokenExpiry; await dbContext.SaveChangesAsync(cancellationToken); return true; }
    public async Task<bool> DeleteAsync(int employeeId, CancellationToken cancellationToken = default) { var e = await dbContext.Auths.SingleOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken); if (e is null) return false; dbContext.Auths.Remove(e); await dbContext.SaveChangesAsync(cancellationToken); return true; }
}
