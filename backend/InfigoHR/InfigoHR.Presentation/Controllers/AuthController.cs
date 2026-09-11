using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/auth-records")]
public sealed class AuthController(IAuthService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuthDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")] 
    public async Task<ActionResult<AuthDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AuthDto>> Create(CreateAuthDto dto, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")] 
    public async Task<IActionResult> Update(int id, UpdateAuthDto dto, CancellationToken cancellationToken) =>
        await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
