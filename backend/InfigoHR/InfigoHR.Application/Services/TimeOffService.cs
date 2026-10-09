using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Services;

public class TimeOffService : ITimeOffService
{
    private readonly ITimeOffRepository _repository;
    private readonly IValidator<CreateTimeOffModel> _createValidator;
    private readonly IValidator<UpdateTimeOffModel> _updateValidator;

    public TimeOffService(
        ITimeOffRepository repository,
        IValidator<CreateTimeOffModel> createValidator,
        IValidator<UpdateTimeOffModel> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<TimeOffModel>> GetAllAsync(int employeeId)
    {
        var requests = await _repository.GetByEmployeeAsync(employeeId);
        return requests.Select(TimeOffMapper.ToModel).ToList();
    }

    public async Task<TimeOffModel> GetByIdAsync(int id, int employeeId)
    {
        var entity = await GetOwnedRequestAsync(id, employeeId);
        return TimeOffMapper.ToModel(entity);
    }

    public async Task<int> CreateAsync(CreateTimeOffModel model, int employeeId)
    {
        await _createValidator.ValidateAndThrowAsync(model);

        var entity = TimeOffMapper.ToEntity(model, employeeId);
        await _repository.AddAsync(entity);

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateTimeOffModel model, int employeeId)
    {
        var entity = await GetOwnedRequestAsync(model.Id, employeeId);
        await _updateValidator.ValidateAndThrowAsync(model);
        EnsurePending(entity);

        TimeOffMapper.ToEntity(model, entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id, int employeeId)
    {
        var entity = await GetOwnedRequestAsync(id, employeeId);
        EnsurePending(entity);
        
        await _repository.DeleteAsync(entity);
    }

    private async Task<LeaveRequestEntity> GetOwnedRequestAsync(int id, int employeeId)
    {
        if (id <= 0)
        {
            throw new Exceptions.ValidationException("Time-off request ID must be greater than zero.");
        }

        var entity = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Time-off request with id {id} was not found.");

        if (entity.EmployeeId != employeeId)
        {
            throw new ForbiddenException("You cannot access another employee's time-off request.");
        }

        return entity;
    }

    private static void EnsurePending(LeaveRequestEntity entity)
    {
        if (entity.Status != LeaveRequestStatus.Pending)
        {
            throw new Exceptions.ValidationException("Only pending time-off requests can be changed or deleted.");
        }
    }
}