using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimeOffController : ControllerBase
{
    private readonly ITimeOffService _service;

    public TimeOffController(ITimeOffService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TimeOffModel>>> GetAll()
    {
        var requests = await _service.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TimeOffModel>> GetById(int id)
    {
        var request = await _service.GetByIdAsync(id);
        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTimeOffModel model)
    {
        var id = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTimeOffModel model)
    {
        if (id != model.Id)
        {
            return BadRequest(new ErrorResponseModel
            {
                Message = "Route ID must match the model ID."
            });
        }

        await _service.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}