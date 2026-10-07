using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.TimeOff;
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
        if (!TryGetEmployeeId(out var employeeId)) return Unauthorized();
        return Ok(await _service.GetForEmployeeAsync(employeeId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TimeOffModel>> GetById(int id)
    {
        if (!TryGetEmployeeId(out var employeeId)) return Unauthorized();
        return Ok(await _service.GetForEmployeeByIdAsync(id, employeeId));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTimeOffModel model)
    {
        if (!TryGetEmployeeId(out var employeeId)) return Unauthorized();
        var id = await _service.CreateAsync(model, employeeId);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTimeOffModel model)
    {
        if (!TryGetEmployeeId(out var employeeId)) return Unauthorized();
        await _service.UpdateAsync(id, model, employeeId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetEmployeeId(out var employeeId)) return Unauthorized();
        await _service.DeleteAsync(id, employeeId);
        return NoContent();
    }

    private bool TryGetEmployeeId(out int employeeId)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(subject, out employeeId) && employeeId > 0;
    }
}
