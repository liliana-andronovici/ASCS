using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.LeaveRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeaveRequestController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestModel>>> GetAll()
    {
        var requests = await _leaveRequestService.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveRequestModel>> GetById(int id)
    {
        var request = await _leaveRequestService.GetByIdAsync(id);
        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateLeaveRequestModel model)
    {
        var id = await _leaveRequestService.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateLeaveRequestModel model)
    {
        await _leaveRequestService.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _leaveRequestService.DeleteAsync(id);
        return NoContent();
    }
}