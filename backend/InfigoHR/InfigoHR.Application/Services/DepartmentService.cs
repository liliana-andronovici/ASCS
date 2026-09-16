using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Departments;

namespace InfigoHR.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IValidator<CreateDepartmentModel> _createValidator;
    private readonly IValidator<UpdateDepartmentModel> _updateValidator;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IValidator<CreateDepartmentModel> createValidator,
        IValidator<UpdateDepartmentModel> updateValidator)
    {
        _departmentRepository = departmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<DepartmentModel>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();
        return departments.Select(DepartmentMapper.ToModel);
    }

    public async Task<DepartmentModel> GetByIdAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null)
        {
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        return DepartmentMapper.ToModel(department);
    }

    public async Task CreateAsync(CreateDepartmentModel model)
    {
        await _createValidator.ValidateAndThrowAsync(model);

        var department = DepartmentMapper.ToEntity(model);
        await _departmentRepository.AddAsync(department);
        await _departmentRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateDepartmentModel model)
    {
        await _updateValidator.ValidateAndThrowAsync(model);

        var department = await _departmentRepository.GetByIdAsync(model.Id);

        if (department is null)
        {
            throw new NotFoundException($"Department with id {model.Id} was not found.");
        }

        DepartmentMapper.Map(model, department);
        await _departmentRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null)
        {
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        _departmentRepository.Remove(department);
        await _departmentRepository.SaveChangesAsync();
    }
}
