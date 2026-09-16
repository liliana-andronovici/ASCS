using InfigoHR.Application.Models;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EmployeeModel>>> GetAll()
    {
        var items = await service.GetAllAsync();
        return Ok(items);
    }

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
    public async Task<ActionResult<EmployeeModel>> Create(
        CreateEmployeeModel dto,
        IValidator<CreateEmployeeModel> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(
                validationResult.ToDictionary()));

        try
        {
            var created = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InfigoHR.Domain.Exceptions.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateEmployeeModel dto,
        IValidator<UpdateEmployeeModel> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(
                validationResult.ToDictionary()));

        try
        {
            await service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
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