using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using InfigoHR.Application.Models.Employees;
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

    public async Task<PagedResult<LocationModel>> GetPagedAsync(LocationParameters paginationParams)
    {
        var pagedEntities = await _repository.SearchAsync(paginationParams);
        return pagedEntities.ToPagedResult(LocationMapper.ToModel);
    }
    
    public async Task<LocationModel> GetByIdAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null)
        {
            throw new NotFoundException($"Location with id {id} was not found.");
        }
        return LocationMapper.ToModel(location);
    }

    public async Task<int> CreateAsync(CreateLocationModel model)
    {
        var location = LocationMapper.ToEntity(model);
        await _repository.AddAsync(location);
        return location.Id;
    }

    public async Task UpdateAsync(UpdateLocationModel model)
    {
        var location = await _repository.GetByIdAsync(model.Id);
        if (location == null)
        {
            throw new NotFoundException($"Location with id {model.Id} was not found.");
        }

        LocationMapper.ToEntity(model, location);
        await _repository.UpdateAsync(location);
    }
    
    public async Task DeleteAsync(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null)
        {
            throw new NotFoundException($"Location with id {id} was not found.");
        }

        await _repository.DeleteAsync(location);
    }

    public async Task<byte[]> ExportCsvAsync()
    {
        var locations = await _repository.GetAllAsync();

        var rows = locations
            .OrderBy(location => location.Id)
            .Select(location => new LocationCsvModel { Name = location.Name });

        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(rows);
        }

        return stream.ToArray();
    }

    public async Task<ImportResultModel> ImportCsvAsync(Stream csvStream)
    {
        const int maxNameLength = 150;

        var result = new ImportResultModel();

        var existingNames = (await _repository.GetAllAsync())
            .Select(location => location.Name.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, configuration);

        if (!await csv.ReadAsync())
            return result;

        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var rowNumber = csv.Parser.Row;

            LocationCsvModel row;
            try
            {
                row = csv.GetRecord<LocationCsvModel>();
            }
            catch (CsvHelperException)
            {
                AddSkippedRow(result, rowNumber, "the row could not be read.");
                continue;
            }

            var name = row.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                AddSkippedRow(result, rowNumber, "Name is required.");
                continue;
            }

            if (name.Length > maxNameLength)
            {
                AddSkippedRow(result, rowNumber, $"Name must not exceed {maxNameLength} characters.");
                continue;
            }

            if (existingNames.Contains(name))
            {
                AddSkippedRow(result, rowNumber, $"Location \"{name}\" already exists.");
                continue;
            }

            await CreateAsync(new CreateLocationModel { Name = name });
            existingNames.Add(name);
            result.Imported++;
        }

        return result;
    }

    private static void AddSkippedRow(ImportResultModel result, int rowNumber, string reason)
    {
        result.Skipped++;
        result.Errors.Add($"Row {rowNumber}: {reason}");
    }
}
