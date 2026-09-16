using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.Repositories;

namespace InfigoHR.Persistence.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository; // changed: repository pattern used instead of DbContext
    }

    public async Task<IEnumerable<DepartmentModel>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();

        // changed: mapper class used
        return departments.Select(DepartmentMapper.ToModel);
    }

    public async Task<DepartmentModel> GetByIdAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id, asNoTracking: true);

        if (department is null)
        {
            // changed: custom exception used instead of returning null
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        return DepartmentMapper.ToModel(department);
    }

    public async Task<DepartmentModel> CreateAsync(CreateDepartmentModel model)
    {
        var department = DepartmentMapper.ToEntity(model); // changed: mapper class used

        await _departmentRepository.AddAsync(department);
        await _departmentRepository.SaveChangesAsync();

        return DepartmentMapper.ToModel(department);
    }

    public async Task<DepartmentModel> UpdateAsync(int id, UpdateDepartmentModel model)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null)
        {
            // changed: custom exception used instead of returning null
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        department.Name = model.Name;
        department.UpdatedOnUtc = DateTime.UtcNow;

        await _departmentRepository.SaveChangesAsync();

        return DepartmentMapper.ToModel(department);
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null)
        {
            // changed: custom exception used instead of returning false
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        _departmentRepository.Remove(department);
        await _departmentRepository.SaveChangesAsync();
    }
}
