using InfigoHR.Application.Models.Locations;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using InfigoHR.Application.Common;
using InfigoHR.Application.QueryParameters.Locations;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocationModel>>> GetAll()
    {
        var locations = await _locationService.GetAllAsync();
        return Ok(locations);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<LocationModel>>> GetPaged([FromQuery] LocationParameters paginationParams)
    {
        var pagedLocations = await _locationService.GetPagedAsync(paginationParams);
        return Ok(pagedLocations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationModel>> GetById(int id)
    {
        var location = await _locationService.GetByIdAsync(id);
        return Ok(location);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var file = await _locationService.ExportCsvAsync();
        return File(file, "text/csv", "locations.csv");
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportResultModel>> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ErrorResponseModel { Message = "Please upload a non-empty CSV file." });

        await using var stream = file.OpenReadStream();
        var result = await _locationService.ImportCsvAsync(stream);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationModel model)
    {
        var id = await _locationService.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateLocationModel model)
    {
        await _locationService.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _locationService.DeleteAsync(id);
        return NoContent();
    }
}