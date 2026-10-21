using System.Security.Claims;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class RoomBookingsController(IRoomService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoomBookingModel>>> GetAll([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken cancellationToken) =>
        Ok(await service.GetBookingsAsync(fromUtc, toUtc, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<object>> Create(CreateRoomBookingModel model, CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirstValue("sub"), out var organizerEmployeeId) || organizerEmployeeId <= 0)
            return Unauthorized();
        var id = await service.CreateBookingAsync(model, organizerEmployeeId, cancellationToken);
        return Ok(new { id });
    }
}
