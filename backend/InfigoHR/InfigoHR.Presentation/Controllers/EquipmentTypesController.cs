using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Equipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class EquipmentTypesController(IEquipmentTypeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EquipmentTypeModel>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }
}
