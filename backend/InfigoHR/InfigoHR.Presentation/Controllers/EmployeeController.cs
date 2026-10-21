using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InfigoHR.Application.QueryParameters.Employees;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeModel>>> GetAll([FromQuery] EmployeeParameters query)
    {
        return Ok(await service.GetAllAsync(query));
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeModel>> GetById(int id)
    {
        var employee = await service.GetByIdAsync(id);
        return Ok(employee);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var file = await service.ExportCsvAsync();
        return File(file, "text/csv", "employees.csv");
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportResultModel>> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ErrorResponseModel { Message = "Please upload a non-empty CSV file." });

        await using var stream = file.OpenReadStream();
        var result = await service.ImportCsvAsync(stream);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/equipment")]
    public async Task<ActionResult<List<EmployeeEquipmentModel>>> GetEquipment(int id)
    {
        return Ok(await service.GetEquipmentAsync(id));
    }

    [AllowAnonymous]
    [HttpPost("{id:int}/equipment")]
    public async Task<ActionResult<int>> AddEquipment(int id, CreateEmployeeEquipmentModel model)
    {
        return Ok(await service.AddEquipmentAsync(id, model));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEmployeeModel model)
    {
        var id = await service.CreateAsync(model);
        return Ok(id);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateEmployeeModel model)
    {
        await service.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
