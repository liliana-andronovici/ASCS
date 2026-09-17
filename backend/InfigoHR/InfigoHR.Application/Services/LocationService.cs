using InfigoHR.Application.Models.Locations;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Domain.Entities;
using InfigoHR.Application.Common;

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

    public async Task<PagedResult<LocationModel>> GetPagedAsync(PaginationParams paginationParams)
    {
        var pagedEntities = await _repository.GetPagedAsync(paginationParams);

        return new PagedResult<LocationModel>
        {
            Items = pagedEntities.Items.Select(LocationMapper.ToModel).ToList(),
            TotalCount = pagedEntities.TotalCount,
            Page = pagedEntities.Page,
            PageSize = pagedEntities.PageSize
        };
    }

    public async Task<LocationModel> GetByIdAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);
        return LocationMapper.ToModel(location);
    }

    public async Task CreateAsync(CreateLocationModel model)
    {
        var location = new LocationEntity { Name = model.Name };
        await _repository.AddAsync(location);
    }

    public async Task UpdateAsync(int id, UpdateLocationModel model)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);

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