using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Models;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public sealed class EmployeeRepository(AppDbContext dbContext)
    : GenericRepository<EmployeeEntity>(dbContext), IEmployeeRepository
{
    public async Task<(List<EmployeeEntity> Items, int TotalCount)> SearchAsync(EmployeeParameters query)
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

        employees = query.SortBy switch
        {
            EmployeeSortField.FirstName => descending ? employees.OrderByDescending(x => x.FirstName) : employees.OrderBy(x => x.FirstName),
            EmployeeSortField.LastName => descending ? employees.OrderByDescending(x => x.LastName) : employees.OrderBy(x => x.LastName),
            EmployeeSortField.Email => descending ? employees.OrderByDescending(x => x.Email) : employees.OrderBy(x => x.Email),
            EmployeeSortField.StartedOnUtc => descending ? employees.OrderByDescending(x => x.StartedOnUtc) : employees.OrderBy(x => x.StartedOnUtc),
            _ => descending ? employees.OrderByDescending(x => x.Id) : employees.OrderBy(x => x.Id)
        };

        var totalCount = await employees.CountAsync();
        var items = await employees.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await Entities.AnyAsync(x => x.Email == email);

    public async Task<bool> DepartmentExistsAsync(int id) =>
        await Context.Set<Domain.Entities.DepartmentEntity>().AnyAsync(x => x.Id == id);

    public async Task<bool> LocationExistsAsync(int id) =>
        await Context.Set<Domain.Entities.LocationEntity>().AnyAsync(x => x.Id == id);
}