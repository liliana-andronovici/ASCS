using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.TimeOff;
using InfigoHR.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        var employeeId = GetCurrentEmployeeId();
        var requests = await _service.GetAllAsync(employeeId);
        return Ok(requests);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TimeOffModel>> GetById(int id)
    {
        var employeeId = GetCurrentEmployeeId();
        var request = await _service.GetByIdAsync(id, employeeId);
        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTimeOffModel model)
    {
        var employeeId = GetCurrentEmployeeId();
        var id = await _service.CreateAsync(model, employeeId);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTimeOffModel model)
    {
        var employeeId = GetCurrentEmployeeId();

        if (id != model.Id)
        {
            return BadRequest(new ErrorResponseModel
            {
                Message = "Route ID must match the model ID."
            });
        }

        await _service.UpdateAsync(model, employeeId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employeeId = GetCurrentEmployeeId();
        await _service.DeleteAsync(id, employeeId);
        return NoContent();
    }

    private int GetCurrentEmployeeId()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (User.Identity?.IsAuthenticated != true
            || !int.TryParse(subject, out var employeeId)
            || employeeId <= 0)
        {
            throw new UnauthorizedException("A valid authenticated employee is required.");
        }

        return employeeId;
    }
}