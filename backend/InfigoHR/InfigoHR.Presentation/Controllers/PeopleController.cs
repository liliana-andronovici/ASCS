using System.Security.Claims;
using System.Text.Json;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Authentication.Authorization;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/people")]
public class PeopleController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public PeopleController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    private EmployeeAccess? GetAccess()
    {
        if (!int.TryParse(User.FindFirstValue("sub"), out var employeeId) || employeeId <= 0)
        {
            return null;
        }

        var canManageEmployees = User.HasClaim("permissions", nameof(Permission.ManageEmployee));
        return new EmployeeAccess(employeeId, canManageEmployees);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Profile(int id, CancellationToken ct) =>
        Ok(await _employeeService.GetProfileAsync(id, ct));

    [HttpPatch("{id:int}"), HasPermission(Permission.ManageEmployee)]
    [RequestSizeLimit(32_768)]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonElement body, CancellationToken ct)
    {
        var access = GetAccess();
        return access is null
            ? Forbid()
            : Ok(await _employeeService.PatchAsync(id, body, access, ct));
    }

    [HttpGet("{id:int}/documents")]
    public async Task<IActionResult> Documents(int id, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        var access = GetAccess();
        return access is null
            ? Forbid()
            : Ok(await _employeeService.GetDocumentsAsync(id, page, pageSize, access, ct));
    }

    [HttpGet("{id:int}/history")]
    public async Task<IActionResult> History(int id, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        var access = GetAccess();
        return access is null
            ? Forbid()
            : Ok(await _employeeService.GetHistoryAsync(id, page, pageSize, access, ct));
    }
}
