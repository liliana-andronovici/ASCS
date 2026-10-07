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

    public async Task<IReadOnlyList<TimeOffModel>> GetForEmployeeAsync(int employeeId)
    {
        var requests = await _repository.GetForEmployeeAsync(employeeId);
        return requests.Select(TimeOffMapper.ToModel).ToList();
    }

    public async Task<TimeOffModel> GetForEmployeeByIdAsync(int id, int employeeId) =>
        TimeOffMapper.ToModel(await GetOwnedRequestAsync(id, employeeId));

    public async Task<int> CreateAsync(CreateTimeOffModel model, int employeeId)
    {
        await _createValidator.ValidateAndThrowAsync(model);
        var entity = TimeOffMapper.ToEntity(model, employeeId);
        await _repository.AddAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(int id, UpdateTimeOffModel model, int employeeId)
    {
        await _updateValidator.ValidateAndThrowAsync(model);
        var entity = await GetOwnedRequestAsync(id, employeeId);
        EnsurePending(entity);
        TimeOffMapper.Apply(model, entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id, int employeeId)
    {
        var entity = await GetOwnedRequestAsync(id, employeeId);
        EnsurePending(entity);
        await _repository.DeleteAsync(entity);
    }

    private async Task<TimeOffEntity> GetOwnedRequestAsync(int id, int employeeId) =>
        await _repository.GetForEmployeeByIdAsync(id, employeeId)
        ?? throw new NotFoundException($"Time-off request with id {id} was not found.");

    private static void EnsurePending(TimeOffEntity entity)
    {
        if (entity.Status != TimeOffStatus.Pending)
        {
            throw new InfigoHR.Application.Exceptions.ValidationException("Only pending time-off requests can be changed or deleted.");
        }
    }
}
