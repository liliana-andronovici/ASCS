using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace InfigoHR.Presentation.Controllers;
[ApiController]
[Route("api/trusted-devices")]
public sealed class TrustedDevicesController(ITrustedDeviceService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<TrustedDeviceDto>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{deviceToken}")] public async Task<ActionResult<TrustedDeviceDto>> GetById(string deviceToken, CancellationToken ct) { var item = await service.GetByIdAsync(deviceToken, ct); return item is null ? NotFound() : Ok(item); }
    [HttpPost] public async Task<ActionResult<TrustedDeviceDto>> Create(CreateTrustedDeviceDto dto, CancellationToken ct) { var created = await service.CreateAsync(dto, ct); return CreatedAtAction(nameof(GetById), new { deviceToken = created.DeviceToken }, created); }
    [HttpPut("{deviceToken}")] public async Task<IActionResult> Update(string deviceToken, UpdateTrustedDeviceDto dto, CancellationToken ct) => await service.UpdateAsync(deviceToken, dto, ct) ? NoContent() : NotFound();
    [HttpDelete("{deviceToken}")] public async Task<IActionResult> Delete(string deviceToken, CancellationToken ct) => await service.DeleteAsync(deviceToken, ct) ? NoContent() : NotFound();
}
