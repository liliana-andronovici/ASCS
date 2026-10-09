using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Interfaces.Services;

public interface ITimeOffService
{
    Task<IReadOnlyList<TimeOffModel>> GetAllAsync(int employeeId);
    Task<TimeOffModel> GetByIdAsync(int id, int employeeId);
    Task<int> CreateAsync(CreateTimeOffModel model, int employeeId);
    Task UpdateAsync(UpdateTimeOffModel model, int employeeId);
    Task DeleteAsync(int id, int employeeId);
}