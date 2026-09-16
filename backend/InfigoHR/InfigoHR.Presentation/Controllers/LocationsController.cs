using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        => Ok(await _locationService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationModel>> GetById(int id)
        => Ok(await _locationService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<LocationModel>> Create(CreateLocationModel model)
    {
        var created = await _locationService.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateLocationModel model)
    {
        await _locationService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _locationService.DeleteAsync(id);
        return NoContent();
    }
}