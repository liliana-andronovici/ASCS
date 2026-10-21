using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IEquipmentRepository : IGenericRepository<EquipmentEntity>
{
    Task<List<EquipmentEntity>> GetAllWithTypesAsync();
    Task<HashSet<int>> GetAssignedIdsAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    Task<bool> HasAssignmentsAsync(int equipmentId);
}
