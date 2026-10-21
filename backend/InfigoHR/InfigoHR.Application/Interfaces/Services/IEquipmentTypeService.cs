using InfigoHR.Application.Models.Equipment;

namespace InfigoHR.Application.Interfaces.Services;

public interface IEquipmentTypeService
{
    Task<List<EquipmentTypeModel>> GetAllAsync();
    Task<int> CreateAsync(EquipmentTypeInputModel model);
}
