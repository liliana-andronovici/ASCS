using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace InfigoHR.Presentation.Controllers;
[ApiController]
[Route("api/leave-requests")]
public sealed class LeaveRequestsController(ILeaveRequestService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<ActionResult<LeaveRequestDto>> GetById(int id, CancellationToken ct) { var item = await service.GetByIdAsync(id, ct); return item is null ? NotFound() : Ok(item); }
    [HttpPost] public async Task<ActionResult<LeaveRequestDto>> Create(CreateLeaveRequestDto dto, CancellationToken ct) { var created = await service.CreateAsync(dto, ct); return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdateLeaveRequestDto dto, CancellationToken ct) => await service.UpdateAsync(id, dto, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
