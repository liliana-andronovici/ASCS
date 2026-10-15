using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IEquipmentTypeRepository : IGenericRepository<EquipmentTypeEntity>
{
    Task<bool> NameExistsAsync(string name);
}
