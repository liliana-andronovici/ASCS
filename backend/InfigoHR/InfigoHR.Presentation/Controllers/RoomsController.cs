using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class RoomsController(IRoomService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoomModel>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetRoomsAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<object>> Create(CreateRoomModel model, CancellationToken cancellationToken)
    {
        var id = await service.CreateRoomAsync(model, cancellationToken);
        return Ok(new { id });
    }
}
