using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace InfigoHR.Presentation.Controllers;
[ApiController]
[Route("api/two-factor-auth")]
public sealed class TwoFactorAuthController(ITwoFactorAuthService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<TwoFactorAuthDto>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{employeeId:int}")] public async Task<ActionResult<TwoFactorAuthDto>> GetById(int employeeId, CancellationToken ct) { var item = await service.GetByIdAsync(employeeId, ct); return item is null ? NotFound() : Ok(item); }
    [HttpPost] public async Task<ActionResult<TwoFactorAuthDto>> Create(CreateTwoFactorAuthDto dto, CancellationToken ct) { var created = await service.CreateAsync(dto, ct); return CreatedAtAction(nameof(GetById), new { employeeId = created.EmployeeId }, created); }
    [HttpPut("{employeeId:int}")] public async Task<IActionResult> Update(int employeeId, UpdateTwoFactorAuthDto dto, CancellationToken ct) => await service.UpdateAsync(employeeId, dto, ct) ? NoContent() : NotFound();
    [HttpDelete("{employeeId:int}")] public async Task<IActionResult> Delete(int employeeId, CancellationToken ct) => await service.DeleteAsync(employeeId, ct) ? NoContent() : NotFound();
}
