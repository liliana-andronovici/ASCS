using System.IdentityModel.Tokens.Jwt;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.TimeOff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TimeOffController(ITimeOffService service) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<TimeOffOverviewModel>> GetOverview()
    {
        return Ok(await service.GetOverviewAsync(GetCurrentEmployeeId()));
    }

    [HttpGet("requests")]
    public async Task<ActionResult<List<TimeOffRequestModel>>> GetMyRequests()
    {
        return Ok(await service.GetMyRequestsAsync(GetCurrentEmployeeId()));
    }

    [HttpPost("requests/preview")]
    public async Task<ActionResult<TimeOffPreviewModel>> Preview(TimeOffPreviewInputModel model)
    {
        return Ok(await service.PreviewAsync(GetCurrentEmployeeId(), model));
    }

    [HttpPost("requests")]
    public async Task<ActionResult<int>> Create(CreateTimeOffRequestModel model)
    {
        var id = await service.CreateAsync(GetCurrentEmployeeId(), model);
        return Ok(id);
    }

    [HttpPost("requests/withdraw")]
    public async Task<IActionResult> Withdraw(WithdrawTimeOffRequestModel model)
    {
        await service.WithdrawAsync(GetCurrentEmployeeId(), model.Id);
        return NoContent();
    }

    private int GetCurrentEmployeeId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.Parse(sub!);
    }
}