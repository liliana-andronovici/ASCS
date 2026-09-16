using InfigoHR.Application.DTOs;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces;
using InfigoHR.Application.Mappers;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Persistence.Services;

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

    public async Task<LocationModel> GetByIdAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);
        return LocationMapper.ToModel(location);
    }

    public async Task<LocationModel> CreateAsync(CreateLocationModel model)
    {
        var location = new LocationEntity { Name = model.Name };
        await _repository.AddAsync(location);
        await _repository.SaveChangesAsync();
        return LocationMapper.ToModel(location);
    }

    public async Task<LocationModel> UpdateAsync(int id, UpdateLocationModel model)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);

        location.Name = model.Name;
        location.UpdatedOnUtc = DateTime.UtcNow;
        await _repository.SaveChangesAsync();

        return LocationMapper.ToModel(location);
    }

    public async Task DeleteAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) throw new LocationNotFoundException(id);

        _repository.Remove(location);
        await _repository.SaveChangesAsync();
    }
}