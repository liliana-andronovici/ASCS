using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Authentication.Authorization;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[HasPermission(Permission.ManageRoles)]
[Route("api/[controller]")]
public class PermissionsController(IPermissionService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAllAsync());
}
