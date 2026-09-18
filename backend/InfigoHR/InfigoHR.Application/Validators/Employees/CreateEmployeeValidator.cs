using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Models;

namespace InfigoHR.Application.Validators;

public class CreateEmployeeModelValidator : AbstractValidator<CreateEmployeeModel>
{
    public CreateEmployeeModelValidator(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(150)
            .MustAsync(async (email, ct) => !await employeeRepository.EmailExistsAsync(email))
            .WithMessage(x => $"An employee with email '{x.Email}' already exists.");

        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.StartedOnUtc).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();

        RuleFor(x => x.DepartmentId)
            .MustAsync(async (id, ct) => id is null || await departmentRepository.ExistsAsync(id.Value))
            .WithMessage("The specified department does not exist.");

        RuleFor(x => x.ManagerId)
            .MustAsync(async (id, ct) => id is null || await employeeRepository.GetByIdAsync(id.Value) is not null)
            .WithMessage("The specified manager does not exist.");
    }
}