using InfigoHR.Application.DTOs;
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
}