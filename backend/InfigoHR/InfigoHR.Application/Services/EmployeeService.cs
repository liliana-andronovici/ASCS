using FluentValidation;
using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models;
using InfigoHR.Application.QueryParameters.Employees;

namespace InfigoHR.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IValidator<CreateEmployeeModel> _createValidator;
    private readonly IValidator<UpdateEmployeeModel> _updateValidator;
    private readonly InvitationLink _invitationLink;

    public EmployeeService(
        IEmployeeRepository repository,
        IValidator<CreateEmployeeModel> createValidator,
        IValidator<UpdateEmployeeModel> updateValidator,
        InvitationLink invitationLink)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _invitationLink = invitationLink;
    }

    public async Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query)
    {
        var pagedList = await _repository.SearchAsync(query);
        return pagedList.ToPagedResult(EmployeeMapper.ToModel);
    }

    public async Task<EmployeeModel> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");

        return EmployeeMapper.ToModel(entity);
    }

    public async Task<int> CreateAsync(CreateEmployeeModel model)
    {
        await _createValidator.ValidateAndThrowAsync(model);

        var entity = EmployeeMapper.ToEntity(model);

        await _repository.AddAsync(entity);

        // Send the employee invitation email after the employee has been created.
        await _invitationLink.SendAsync(
            entity.Email,
            "http://localhost:3000/register",
            entity.Id.ToString());

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateEmployeeModel model)
    {
        await _updateValidator.ValidateAndThrowAsync(model);

        var entity = await _repository.GetByIdAsync(model.Id)
                     ?? throw new NotFoundException($"Employee with id {model.Id} was not found.");

        EmployeeMapper.ToEntity(model, entity);

        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");

                await _repository.DeleteAsync(entity);
    }
}