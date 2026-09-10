using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace InfigoHR.Persistence.Services;
public sealed class TrustedDeviceService(AppDbContext dbContext) : ITrustedDeviceService
{
    public async Task<IReadOnlyList<TrustedDeviceDto>> GetAllAsync(CancellationToken cancellationToken = default) => await dbContext.TrustedDevices.AsNoTracking().OrderBy(x => x.DeviceToken).Select(x => new TrustedDeviceDto(x.DeviceToken, x.EmployeeId, x.TrustedUntil)).ToListAsync(cancellationToken);
    public async Task<TrustedDeviceDto?> GetByIdAsync(string deviceToken, CancellationToken cancellationToken = default) => await dbContext.TrustedDevices.AsNoTracking().Where(x => x.DeviceToken == deviceToken).Select(x => new TrustedDeviceDto(x.DeviceToken, x.EmployeeId, x.TrustedUntil)).SingleOrDefaultAsync(cancellationToken);
    public async Task<TrustedDeviceDto> CreateAsync(CreateTrustedDeviceDto dto, CancellationToken cancellationToken = default) { var e = new TrustedDevice { DeviceToken = dto.DeviceToken, EmployeeId = dto.EmployeeId, TrustedUntil = dto.TrustedUntil }; dbContext.TrustedDevices.Add(e); await dbContext.SaveChangesAsync(cancellationToken); return new TrustedDeviceDto(e.DeviceToken, e.EmployeeId, e.TrustedUntil); }
    public async Task<bool> UpdateAsync(string deviceToken, UpdateTrustedDeviceDto dto, CancellationToken cancellationToken = default) { var e = await dbContext.TrustedDevices.SingleOrDefaultAsync(x => x.DeviceToken == deviceToken, cancellationToken); if (e is null) return false; e.EmployeeId = dto.EmployeeId; e.TrustedUntil = dto.TrustedUntil; await dbContext.SaveChangesAsync(cancellationToken); return true; }
    public async Task<bool> DeleteAsync(string deviceToken, CancellationToken cancellationToken = default) { var e = await dbContext.TrustedDevices.SingleOrDefaultAsync(x => x.DeviceToken == deviceToken, cancellationToken); if (e is null) return false; dbContext.TrustedDevices.Remove(e); await dbContext.SaveChangesAsync(cancellationToken); return true; }
}
