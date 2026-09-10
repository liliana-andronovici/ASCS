using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace InfigoHR.Persistence.Services;
public sealed class LeaveRequestService(AppDbContext dbContext) : ILeaveRequestService
{
    public async Task<IReadOnlyList<LeaveRequestDto>> GetAllAsync(CancellationToken cancellationToken = default) => await dbContext.LeaveRequests.AsNoTracking().OrderBy(x => x.Id).Select(x => new LeaveRequestDto(x.Id, x.EmployeeId, x.StartDate, x.EndDate, x.Note, x.Type, x.StatusT)).ToListAsync(cancellationToken);
    public async Task<LeaveRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await dbContext.LeaveRequests.AsNoTracking().Where(x => x.Id == id).Select(x => new LeaveRequestDto(x.Id, x.EmployeeId, x.StartDate, x.EndDate, x.Note, x.Type, x.StatusT)).SingleOrDefaultAsync(cancellationToken);
    public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken = default) { var e = new LeaveRequest { EmployeeId = dto.EmployeeId, StartDate = dto.StartDate, EndDate = dto.EndDate, Note = dto.Note, Type = dto.Type, StatusT = dto.Status }; dbContext.LeaveRequests.Add(e); await dbContext.SaveChangesAsync(cancellationToken); return new LeaveRequestDto(e.Id, e.EmployeeId, e.StartDate, e.EndDate, e.Note, e.Type, e.StatusT); }
    public async Task<bool> UpdateAsync(int id, UpdateLeaveRequestDto dto, CancellationToken cancellationToken = default) { var e = await dbContext.LeaveRequests.SingleOrDefaultAsync(x => x.Id == id, cancellationToken); if (e is null) return false; e.EmployeeId = dto.EmployeeId; e.StartDate = dto.StartDate; e.EndDate = dto.EndDate; e.Note = dto.Note; e.Type = dto.Type; e.StatusT = dto.Status; await dbContext.SaveChangesAsync(cancellationToken); return true; }
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) { var e = await dbContext.LeaveRequests.SingleOrDefaultAsync(x => x.Id == id, cancellationToken); if (e is null) return false; dbContext.LeaveRequests.Remove(e); await dbContext.SaveChangesAsync(cancellationToken); return true; }
}
