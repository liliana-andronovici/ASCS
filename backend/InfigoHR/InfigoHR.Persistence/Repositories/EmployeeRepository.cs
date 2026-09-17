using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public sealed class EmployeeRepository(AppDbContext dbContext)
    : GenericRepository<EmployeeEntity>(dbContext), IEmployeeRepository
{
    public async Task<bool> EmailExistsAsync(string email) =>
        await Entities.AnyAsync(x => x.Email == email);

    public async Task<bool> DepartmentExistsAsync(int id) =>
        await Context.Set<Domain.Entities.DepartmentEntity>().AnyAsync(x => x.Id == id);

    public async Task<bool> LocationExistsAsync(int id) =>
        await Context.Set<Domain.Entities.LocationEntity>().AnyAsync(x => x.Id == id);
}