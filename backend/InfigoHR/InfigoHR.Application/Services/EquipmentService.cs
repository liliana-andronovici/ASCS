using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Equipment;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public class EquipmentService(
    IEquipmentRepository repository,
    IEquipmentTypeRepository typeRepository,
    IValidator<EquipmentInputModel> validator) : IEquipmentService
{
    public async Task<List<EquipmentModel>> GetAllAsync()
    {
        var equipment = await repository.GetAllWithTypesAsync();
        var assignedIds = await repository.GetAssignedIdsAsync();
        return equipment.OrderBy(item => item.Name).ThenBy(item => item.Code)
            .Select(item => new EquipmentModel
            {
                Id = item.Id,
                Name = item.Name,
                Code = item.Code,
                EquipmentTypeId = item.EquipmentTypeId,
                TypeName = item.EquipmentType.Name,
                IsAssigned = assignedIds.Contains(item.Id)
            })
            .ToList();
    }

    public async Task<int> CreateAsync(EquipmentInputModel model)
    {
        await validator.ValidateAndThrowAsync(model);
        var name = model.Name.Trim();
        var code = model.Code.Trim();

        if (await typeRepository.GetByIdAsync(model.EquipmentTypeId) is null)
            throw new NotFoundException($"Asset type with id {model.EquipmentTypeId} was not found.");
        if (await repository.CodeExistsAsync(code))
            throw new ConflictException($"Asset code '{code}' already exists.");

        var equipment = new EquipmentEntity { Name = name, Code = code, EquipmentTypeId = model.EquipmentTypeId };
        await repository.AddAsync(equipment);
        return equipment.Id;
    }

    public async Task UpdateAsync(int id, EquipmentInputModel model)
    {
        await validator.ValidateAndThrowAsync(model);
        var equipment = await repository.GetByIdAsync(id)
                        ?? throw new NotFoundException($"Asset with id {id} was not found.");
        var name = model.Name.Trim();
        var code = model.Code.Trim();

        if (await typeRepository.GetByIdAsync(model.EquipmentTypeId) is null)
            throw new NotFoundException($"Asset type with id {model.EquipmentTypeId} was not found.");
        if (await repository.CodeExistsAsync(code, id))
            throw new ConflictException($"Asset code '{code}' already exists.");

        equipment.Name = name;
        equipment.Code = code;
        equipment.EquipmentTypeId = model.EquipmentTypeId;
        await repository.UpdateAsync(equipment);
    }

    public async Task DeleteAsync(int id)
    {
        var equipment = await repository.GetByIdAsync(id)
                        ?? throw new NotFoundException($"Asset with id {id} was not found.");

        if (await repository.HasAssignmentsAsync(id))
            throw new ConflictException("Assets with assignment history cannot be deleted.");

        await repository.DeleteAsync(equipment);
    }
}
