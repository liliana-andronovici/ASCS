using FluentValidation;
using Hangfire;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Equipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class EquipmentTypeJobsController(
    IBackgroundJobClient jobs,
    IValidator<EquipmentTypeInputModel> validator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(EquipmentTypeInputModel model)
    {
        await validator.ValidateAndThrowAsync(model);
        model.Name = model.Name.Trim();
        var jobId = jobs.Enqueue<IEquipmentTypeService>(service => service.CreateAsync(model));
        return Accepted(new { jobId });
    }
}
