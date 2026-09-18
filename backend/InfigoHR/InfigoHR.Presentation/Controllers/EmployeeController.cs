using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using InfigoHR.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeModel>>> GetAll([FromQuery] EmployeeParameters query) =>
        Ok(await service.GetAllAsync(query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeModel>> GetById(int id)
    {
        try
        {
            return Ok(await service.GetByIdAsync(id));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeModel model)
    {
        try
        {
            await service.CreateAsync(model);
            return NoContent();
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InfigoHR.Domain.Exceptions.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeModel model)
    {
        model.Id = id;
        try
        {
            await service.UpdateAsync(model);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InfigoHR.Domain.Exceptions.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}