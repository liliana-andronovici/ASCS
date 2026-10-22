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
    private readonly ILeaveRequestService _service;

    public TimeOffController(ILeaveRequestService service)
    {
        _service = service;
    }

    [HttpGet("out-today")]
    public async Task<IActionResult> GetOutToday([FromQuery] DateOnly date)
    {
        if (date == default) return BadRequest("A date is required.");
        return Ok(new { count = await _service.GetOutTodayCountAsync(date) });
    }

    [HttpGet("overview")]
    public async Task<ActionResult<TimeOffOverviewModel>> GetOverview() =>
        Ok(await _service.GetOverviewAsync());

    [HttpGet("requests")]
    public async Task<ActionResult<IReadOnlyList<TimeOffRequestSummaryModel>>> GetRequests() =>
        Ok(await _service.GetRequestSummariesAsync());

    [HttpPost("requests/preview")]
    public async Task<ActionResult<TimeOffPreviewModel>> Preview(TimeOffPreviewInputModel model) =>
        Ok(await _service.PreviewAsync(model));

    [HttpPost("requests")]
    public async Task<ActionResult<int>> Create(TimeOffRequestInputModel model) =>
        Ok(await _service.CreateFromPageAsync(model));

    [HttpPost("requests/withdraw")]
    public async Task<IActionResult> Withdraw(WithdrawTimeOffModel model)
    {
        await _service.WithdrawAsync(model.Id);
        return NoContent();
    }
}
