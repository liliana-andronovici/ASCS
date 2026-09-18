using InfigoHR.Application.Models.Locations;
using InfigoHR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using InfigoHR.Application.Common;

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
    public async Task<ActionResult<PagedListModel<LocationModel>>> GetPaged([FromQuery] PaginationParams paginationParams)
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