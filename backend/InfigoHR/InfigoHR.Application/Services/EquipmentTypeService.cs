using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Equipment;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public class EquipmentTypeService(
    IEquipmentTypeRepository repository,
    IValidator<EquipmentTypeInputModel> validator) : IEquipmentTypeService
{
    public async Task<List<EquipmentTypeModel>> GetAllAsync()
    {
        var types = await repository.GetAllAsync();
        return types.OrderBy(type => type.Name)
            .Select(type => new EquipmentTypeModel { Id = type.Id, Name = type.Name })
            .ToList();
    }

    public async Task<int> CreateAsync(EquipmentTypeInputModel model)
    {
        await validator.ValidateAndThrowAsync(model);
        var name = model.Name.Trim();
        if (await repository.NameExistsAsync(name))
            throw new ConflictException($"Asset type '{name}' already exists.");

        var type = new EquipmentTypeEntity { Name = name };
        await repository.AddAsync(type);
        return type.Id;
    }
}
