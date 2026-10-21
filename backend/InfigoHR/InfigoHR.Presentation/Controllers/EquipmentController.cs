using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Equipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class EquipmentController(IEquipmentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EquipmentModel>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(EquipmentInputModel model)
    {
        return Ok(await service.CreateAsync(model));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EquipmentInputModel model)
    {
        await service.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
