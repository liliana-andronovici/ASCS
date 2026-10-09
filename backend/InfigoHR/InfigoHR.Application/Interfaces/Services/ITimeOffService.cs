using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Interfaces.Services;

public interface ITimeOffService
{
    Task<IReadOnlyList<TimeOffModel>> GetAllAsync();
    Task<TimeOffModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateTimeOffModel model);
    Task UpdateAsync(UpdateTimeOffModel model);
    Task DeleteAsync(int id);
}