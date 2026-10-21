using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class EquipmentRepository(AppDbContext context)
    : GenericRepository<EquipmentEntity>(context), IEquipmentRepository
{
    public Task<List<EquipmentEntity>> GetAllWithTypesAsync() =>
        Entities.AsNoTracking().Include(item => item.EquipmentType).ToListAsync();

    public async Task<HashSet<int>> GetAssignedIdsAsync() =>
        (await Context.Set<EmployeeEquipmentEntity>()
            .Where(assignment => assignment.ReturnedOnUtc == null)
            .Select(assignment => assignment.EquipmentId)
            .ToListAsync()).ToHashSet();

    public Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var normalizedCode = code.ToLower();
        return Entities.AnyAsync(item =>
            item.Code.ToLower() == normalizedCode &&
            (excludeId == null || item.Id != excludeId));
    }

    public Task<bool> HasAssignmentsAsync(int equipmentId) =>
        Context.Set<EmployeeEquipmentEntity>()
            .AnyAsync(assignment => assignment.EquipmentId == equipmentId);
}
