using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class EquipmentTypeRepository(AppDbContext context)
    : GenericRepository<EquipmentTypeEntity>(context), IEquipmentTypeRepository
{
    public Task<bool> NameExistsAsync(string name)
    {
        var normalizedName = name.ToLower();
        return Entities.AnyAsync(item => item.Name.ToLower() == normalizedName);
    }
}
