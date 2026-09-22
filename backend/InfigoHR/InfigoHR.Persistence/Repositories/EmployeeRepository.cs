using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;
using InfigoHR.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using InfigoHR.Application.Models;
using InfigoHR.Application.QueryParameters.Employees;

namespace InfigoHR.Persistence.Repositories;

public class EmployeeRepository(AppDbContext dbContext)
    : GenericRepository<EmployeeEntity>(dbContext), IEmployeeRepository
{
    public async Task<PagedList<EmployeeEntity>> SearchAsync(EmployeeParameters query)
    {
        var employees = Entities.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            employees = employees.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (query.DepartmentId is not null)
            employees = employees.Where(x => x.DepartmentId == query.DepartmentId);

        if (query.LocationId is not null)
            employees = employees.Where(x => x.LocationId == query.LocationId);

        if (query.Status is not null)
            employees = employees.Where(x => x.Status == query.Status);

        var descending = query.SortDirection == SortDirection.Descending;
        employees = ApplySorting(employees, query.SortBy, descending);

        return await employees.ToPagedListAsync(query.Page, query.PageSize);
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await Entities.AnyAsync(x => x.Email == email);

    private static IQueryable<EmployeeEntity> ApplySorting(
        IQueryable<EmployeeEntity> employees, EmployeeSortField sortBy, bool descending) =>
        sortBy switch
        {
            EmployeeSortField.FirstName => descending ? employees.OrderByDescending(x => x.FirstName) : employees.OrderBy(x => x.FirstName),
            EmployeeSortField.LastName => descending ? employees.OrderByDescending(x => x.LastName) : employees.OrderBy(x => x.LastName),
            EmployeeSortField.Email => descending ? employees.OrderByDescending(x => x.Email) : employees.OrderBy(x => x.Email),
            EmployeeSortField.StartedOnUtc => descending ? employees.OrderByDescending(x => x.StartedOnUtc) : employees.OrderBy(x => x.StartedOnUtc),
            _ => descending ? employees.OrderByDescending(x => x.Id) : employees.OrderBy(x => x.Id)
        };
}