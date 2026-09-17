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
    public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetAll() =>
        Ok(await service.GetAllAsync());

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
            var created = await service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return ValidationProblem(new ValidationProblemDetails(
                ex.Errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage })));
        }
        catch (InfigoHR.Domain.Exceptions.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateEmployeeModel model)
    {
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
            return ValidationProblem(new ValidationProblemDetails(
                ex.Errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage })));
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