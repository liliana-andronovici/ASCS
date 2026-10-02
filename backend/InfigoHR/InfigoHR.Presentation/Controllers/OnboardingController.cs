using System.IdentityModel.Tokens.Jwt;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OnboardingController(IOnboardingService service) : ControllerBase
{
    [HttpGet("checklist")]
    public async Task<ActionResult<OnboardingChecklistModel>> GetChecklist()
    {
        return Ok(await service.GetChecklistAsync(GetCurrentEmployeeId()));
    }

    [HttpPost("tasks")]
    public async Task<IActionResult> AddTask(AddOnboardingTaskModel model)
    {
        await service.AddTaskAsync(GetCurrentEmployeeId(), model);
        return NoContent();
    }

    [HttpPost("tasks/{taskId:int}/toggle")]
    public async Task<IActionResult> ToggleTask(int taskId)
    {
        await service.ToggleTaskAsync(GetCurrentEmployeeId(), taskId);
        return NoContent();
    }

    private int GetCurrentEmployeeId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.Parse(sub!);
    }
}