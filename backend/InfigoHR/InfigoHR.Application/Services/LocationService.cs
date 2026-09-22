using InfigoHR.Application.Models.Locations;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Domain.Entities;
using InfigoHR.Application.Common;
using InfigoHR.Application.QueryParameters.Locations;

namespace InfigoHR.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _repository;

    public LocationService(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LocationModel>> GetAllAsync()
    {
        var locations = await _repository.GetAllAsync();
        return locations.Select(LocationMapper.ToModel).ToList();
    }

    public async Task<PagedListModel<LocationModel>> GetPagedAsync(PaginationParams paginationParams)
    {
        var pagedEntities = await _repository.GetPagedAsync(paginationParams);
        return pagedEntities.ToPagedModel(LocationMapper.ToModel);
    }
    
    public async Task<LocationModel> GetByIdAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);
        return LocationMapper.ToModel(location);
    }

    public async Task<int> CreateAsync(CreateLocationModel model)
    {
        var location = new LocationEntity { Name = model.Name };
        await _repository.AddAsync(location);
        return location.Id;
    }

    public async Task UpdateAsync(UpdateLocationModel model)
    {
        var location = await _repository.GetByIdAsync(model.Id);
        if (location == null) throw new LocationNotFoundException(model.Id);

        LocationMapper.ToEntity(model, location);
        await _repository.UpdateAsync(location);
    }
    
    public async Task DeleteAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);

        await _repository.DeleteAsync(location);
    }
}