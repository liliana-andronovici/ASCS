using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Interfaces.Services;

public interface ITimeOffService
{
    Task<IReadOnlyList<TimeOffModel>> GetForEmployeeAsync(int employeeId);
    Task<TimeOffModel> GetForEmployeeByIdAsync(int id, int employeeId);
    Task<int> CreateAsync(CreateTimeOffModel model, int employeeId);
    Task UpdateAsync(int id, UpdateTimeOffModel model, int employeeId);
    Task DeleteAsync(int id, int employeeId);
}
