using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.QueryParameters.Employees;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;
using InfigoHR.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using InfigoHR.Application.Models;

namespace InfigoHR.Persistence.Repositories;

public class EmployeeRepository
    : GenericRepository<EmployeeEntity>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<EmployeeEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return Entities
            .AsNoTracking()
            .Include(employee => employee.Auth)
            .Include(employee => employee.Roles)
            .ThenInclude(role => role.Permissions)
            .SingleOrDefaultAsync(
                employee => employee.Email == email,
                cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(
            employee => employee.Email == email,
            cancellationToken);
    }

    public Task<bool> ExistsAsync(int id) =>
        Entities.AnyAsync(employee => employee.Id == id);

    public async Task<PagedList<EmployeeEntity>> SearchAsync(
        EmployeeParameters query)
    {
        var employees = ApplyFilters(Entities.AsNoTracking().AsQueryable(), query);

        var descending = query.SortDirection == SortDirection.Descending;
        employees = ApplySorting(employees, query.SortBy, descending);

        return await employees.ToPagedListAsync(query.Page, query.PageSize);
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await Entities.AnyAsync(x => x.Email == email);

    private static IQueryable<EmployeeEntity> ApplyFilters(
        IQueryable<EmployeeEntity> employees, EmployeeParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            employees = employees.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (query.DepartmentId is not null)
            employees = employees.Where(x =>
                x.DepartmentId == query.DepartmentId);

        if (query.LocationId is not null)
            employees = employees.Where(x =>
                x.LocationId == query.LocationId);

        if (query.Status is not null)
            employees = employees.Where(x =>
                x.Status == query.Status);

        return employees;
    }

    private static IQueryable<EmployeeEntity> ApplySorting(
        IQueryable<EmployeeEntity> employees,
        EmployeeSortField sortBy,
        bool descending) =>
        sortBy switch
        {
            EmployeeSortField.FirstName => descending
                ? employees.OrderByDescending(x => x.FirstName)
                : employees.OrderBy(x => x.FirstName),

            EmployeeSortField.LastName => descending
                ? employees.OrderByDescending(x => x.LastName)
                : employees.OrderBy(x => x.LastName),

            EmployeeSortField.Email => descending
                ? employees.OrderByDescending(x => x.Email)
                : employees.OrderBy(x => x.Email),

            EmployeeSortField.StartedOnUtc => descending
                ? employees.OrderByDescending(x => x.StartedOnUtc)
                : employees.OrderBy(x => x.StartedOnUtc),

            _ => descending
                ? employees.OrderByDescending(x => x.Id)
                : employees.OrderBy(x => x.Id)
        };

    public async Task<List<OnboardingEmployeeModel>> GetAllWithDepartmentAsync() =>
        await Entities
            .AsNoTracking()
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ThenBy(x => x.Id)
            .Select(x => new OnboardingEmployeeModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Title = x.Title,
                Department = x.Department != null ? x.Department.Name : "—",
                StartedOnUtc = x.StartedOnUtc
            })
            .ToListAsync();
}
