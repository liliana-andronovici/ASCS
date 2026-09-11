using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class LeaveRequestService(AppDbContext dbContext) : ILeaveRequestService
{
    public async Task<IReadOnlyList<LeaveRequestDto>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.LeaveRequests.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new LeaveRequestDto(
                x.Id,
                x.EmployeeId,
                x.StartDateUtc,
                x.EndDateUtc,
                x.Note,
                x.Type,
                x.StatusT))
            .ToListAsync(cancellationToken);

    public async Task<LeaveRequestDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await dbContext.LeaveRequests.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new LeaveRequestDto(
                x.Id,
                x.EmployeeId,
                x.StartDateUtc,
                x.EndDateUtc,
                x.Note,
                x.Type,
                x.StatusT))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<LeaveRequestDto> CreateAsync(
        CreateLeaveRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            StartDateUtc = dto.StartDateUtc,
            EndDateUtc = dto.EndDateUtc,
            Note = dto.Note,
            Type = dto.Type,
            StatusT = dto.Status
        };
        dbContext.LeaveRequests.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateLeaveRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.LeaveRequests.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        entity.EmployeeId = dto.EmployeeId;
        entity.StartDateUtc = dto.StartDateUtc;
        entity.EndDateUtc = dto.EndDateUtc;
        entity.Note = dto.Note;
        entity.Type = dto.Type;
        entity.StatusT = dto.Status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.LeaveRequests.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        dbContext.LeaveRequests.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static LeaveRequestDto Map(LeaveRequest entity) =>
        new(
            entity.Id,
            entity.EmployeeId,
            entity.StartDateUtc,
            entity.EndDateUtc,
            entity.Note,
            entity.Type,
            entity.StatusT);
}
