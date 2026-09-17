using InfigoHR.Application.Models.Locations;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Mappers;

public static class LocationMapper
{
    public static LocationModel ToModel(LocationEntity entity)
    {
        return new LocationModel
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    public static void ToEntity(UpdateLocationModel model, LocationEntity entity)
    {
        entity.Name = model.Name;
    }
}