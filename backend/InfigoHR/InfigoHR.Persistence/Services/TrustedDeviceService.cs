using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class TrustedDeviceService(AppDbContext dbContext) : ITrustedDeviceService
{
    public async Task<IReadOnlyList<TrustedDeviceDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.TrustedDevices.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new TrustedDeviceDto(x.Id, x.DeviceToken, x.EmployeeId, x.TrustedUntilUtc))
            .ToListAsync(cancellationToken);

    public async Task<TrustedDeviceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.TrustedDevices.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TrustedDeviceDto(x.Id, x.DeviceToken, x.EmployeeId, x.TrustedUntilUtc))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<TrustedDeviceDto> CreateAsync(
        CreateTrustedDeviceDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new TrustedDevice
        {
            DeviceToken = dto.DeviceToken,
            EmployeeId = dto.EmployeeId,
            TrustedUntilUtc = dto.TrustedUntilUtc
        };
        dbContext.TrustedDevices.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new TrustedDeviceDto(entity.Id, entity.DeviceToken, entity.EmployeeId, entity.TrustedUntilUtc);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateTrustedDeviceDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.TrustedDevices.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.DeviceToken = dto.DeviceToken;
        entity.EmployeeId = dto.EmployeeId;
        entity.TrustedUntilUtc = dto.TrustedUntilUtc;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.TrustedDevices.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        dbContext.TrustedDevices.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
