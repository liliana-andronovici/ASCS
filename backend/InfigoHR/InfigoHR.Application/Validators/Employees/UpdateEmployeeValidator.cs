using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Models;

namespace InfigoHR.Application.Validators;

public class UpdateEmployeeModelValidator : AbstractValidator<UpdateEmployeeModel>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public UpdateEmployeeModelValidator(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.StartedOnUtc).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();

        RuleFor(x => x.DepartmentId)
            .MustAsync(DepartmentExistsAsync)
            .WithMessage("The specified department does not exist.");

        RuleFor(x => x.ManagerId)
            .MustAsync(ManagerExistsAsync)
            .WithMessage("The specified manager does not exist.");
    }

    private async Task<bool> DepartmentExistsAsync(int? departmentId, CancellationToken ct) =>
        departmentId is null || await _departmentRepository.ExistsAsync(departmentId.Value);

    private async Task<bool> ManagerExistsAsync(int? managerId, CancellationToken ct) =>
        managerId is null || await _employeeRepository.GetByIdAsync(managerId.Value) is not null;
}