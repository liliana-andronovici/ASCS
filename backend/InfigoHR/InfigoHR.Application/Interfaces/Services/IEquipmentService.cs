using InfigoHR.Application.Models.Equipment;

namespace InfigoHR.Application.Interfaces.Services;

public interface IEquipmentService
{
    Task<List<EquipmentModel>> GetAllAsync();
    Task<int> CreateAsync(EquipmentInputModel model);
    Task UpdateAsync(int id, EquipmentInputModel model);
    Task DeleteAsync(int id);
}
