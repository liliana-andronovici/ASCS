using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.LeaveRequests;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly IValidator<CreateLeaveRequestModel> _createValidator;
    private readonly IValidator<UpdateLeaveRequestModel> _updateValidator;

    public LeaveRequestService(
        ILeaveRequestRepository leaveRequestRepository,
        ICurrentUserAccessor currentUserAccessor,
        IValidator<CreateLeaveRequestModel> createValidator,
        IValidator<UpdateLeaveRequestModel> updateValidator)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _currentUserAccessor = currentUserAccessor;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<LeaveRequestModel>> GetAllAsync()
    {
        var employeeId = _currentUserAccessor.EmployeeId;
        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId);

        return requests.Select(LeaveRequestMapper.ToModel).ToList();
    }

    public async Task<LeaveRequestModel> GetByIdAsync(int id)
    {
        var entity = await GetOwnedRequestAsync(id);
        return LeaveRequestMapper.ToModel(entity);
    }

    public async Task<int> CreateAsync(CreateLeaveRequestModel model)
    {
        var employeeId = _currentUserAccessor.EmployeeId;
        await _createValidator.ValidateAndThrowAsync(model);

        var entity = LeaveRequestMapper.ToEntity(model, employeeId);
        await _leaveRequestRepository.AddAsync(entity);

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateLeaveRequestModel model)
    {
        await _updateValidator.ValidateAndThrowAsync(model);
        var entity = await GetOwnedRequestAsync(model.Id);
        EnsurePending(entity);

        LeaveRequestMapper.ToEntity(model, entity);
        await _leaveRequestRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetOwnedRequestAsync(id);
        EnsurePending(entity);

        await _leaveRequestRepository.DeleteAsync(entity);
    }

    private async Task<LeaveRequestEntity> GetOwnedRequestAsync(int id)
    {
        var employeeId = _currentUserAccessor.EmployeeId;

        var entity = await _leaveRequestRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Leave request with id {id} was not found.");

        if (entity.EmployeeId != employeeId)
        {
            throw new ForbiddenException("You cannot access another employee's leave request.");
        }

        return entity;
    }

    private static void EnsurePending(LeaveRequestEntity entity)
    {
        if (entity.Status != LeaveRequestStatus.Pending)
        {
            throw new Exceptions.ValidationException("Only pending leave requests can be changed or deleted.");
        }
    }
}
