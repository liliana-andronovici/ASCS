using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/employees/org-chart")]
public class OrgChartController(IOrgChartService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrgChartEmployeeModel>>> GetOrgChart()
    {
        var orgChart = await service.GetOrgChartAsync();
        return Ok(orgChart);
    }
}
