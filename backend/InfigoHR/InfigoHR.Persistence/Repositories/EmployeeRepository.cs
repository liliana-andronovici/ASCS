using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.QueryParameters.Employees;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;
using InfigoHR.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            .Include(employee => employee.TwoFactorAuth)
            .Include(employee => employee.Roles)
            .ThenInclude(role => role.Permissions)
            .SingleOrDefaultAsync(
                employee => employee.Email == email,
                cancellationToken);
    }

    public async Task<List<EmployeeEntity>> GetAllWithDetailsAsync() =>
        await Entities
            .AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.Location)
            .OrderBy(employee => employee.Id)
            .ToListAsync();

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

    public Task<EmployeeEntity?> GetProfileAsync(int id, CancellationToken cancellationToken = default) =>
        Entities.AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.Location)
            .Include(employee => employee.Manager)
            .SingleOrDefaultAsync(employee => employee.Id == id, cancellationToken);

    public Task<List<EmployeeMainDataModel>> GetDirectReportsAsync(int id, CancellationToken cancellationToken = default) =>
        Entities.AsNoTracking()
            .Where(employee => employee.ManagerId == id)
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.Id)
            .Select(employee => new EmployeeMainDataModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title
            })
            .ToListAsync(cancellationToken);

    public async Task UpdateProfileAsync(int id, long expectedVersion, int actorId,
        Func<EmployeeEntity, CancellationToken, Task> update, CancellationToken cancellationToken = default)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
        await Context.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(73003)", cancellationToken);
        var employee = await Entities.SingleOrDefaultAsync(row => row.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");
        if (employee.ProfileVersion != expectedVersion)
            throw new ConflictException("The employee changed; reload and retry.");

        await update(employee, cancellationToken);
        if (employee.ManagerId == id)
            throw new InfigoHR.Application.Exceptions.ValidationException("An employee cannot manage themselves.");
        if (employee.ManagerId is { } managerId)
        {
            var managers = await Entities.AsNoTracking()
                .Select(row => new { row.Id, row.ManagerId })
                .ToDictionaryAsync(row => row.Id, row => row.ManagerId, cancellationToken);
            if (!managers.ContainsKey(managerId))
                throw new InfigoHR.Application.Exceptions.ValidationException("The specified manager does not exist.");
            var visited = new HashSet<int>();
            for (int? current = managerId; current is { } currentId; current = managers.GetValueOrDefault(currentId))
                if (currentId == id || !visited.Add(currentId))
                    throw new InfigoHR.Application.Exceptions.ValidationException("The manager would create a reporting cycle.");
        }

        Context.ChangeTracker.DetectChanges();
        var changedFields = Context.Entry(employee).Properties
            .Where(property => property.IsModified)
            .Select(property => property.Metadata.Name)
            .OrderBy(name => name)
            .ToArray();
        if (changedFields.Length > 0)
        {
            employee.ProfileVersion++;
            employee.UpdatedOnUtc = DateTime.UtcNow;
            Context.EmployeeHistory.Add(new EmployeeHistoryEntity
            {
                EmployeeId = id,
                ActorEmployeeId = actorId,
                ChangedFields = changedFields
            });
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("The employee changed; reload and retry.");
            }
        }
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<PagedResult<EmployeeDocumentModel>> GetDocumentsAsync(int id, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var rows = Context.EmployeeDocuments.AsNoTracking().Where(document => document.EmployeeId == id);
        return new PagedResult<EmployeeDocumentModel>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = await rows.CountAsync(cancellationToken),
            Items = await rows.OrderByDescending(document => document.Id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(document => new EmployeeDocumentModel
                {
                    Id = document.Id,
                    Name = document.Name,
                    FileName = document.FileName,
                    ContentType = document.ContentType,
                    SizeBytes = document.SizeBytes,
                    CreatedOnUtc = document.CreatedOnUtc
                }).ToListAsync(cancellationToken)
        };
    }

    public async Task<PagedResult<EmployeeHistoryModel>> GetHistoryAsync(int id, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var rows = Context.EmployeeHistory.AsNoTracking().Where(history => history.EmployeeId == id);
        return new PagedResult<EmployeeHistoryModel>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = await rows.CountAsync(cancellationToken),
            Items = await rows.OrderByDescending(history => history.Id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(history => new EmployeeHistoryModel
                {
                    Id = history.Id,
                    ActorEmployeeId = history.ActorEmployeeId,
                    ChangedFields = history.ChangedFields,
                    CreatedOnUtc = history.CreatedOnUtc
                }).ToListAsync(cancellationToken)
        };
    }

    public async Task<PagedList<EmployeeEntity>> SearchAsync(
        EmployeeParameters query)
    {
        var employees = ApplyFilters(Entities.AsNoTracking().AsQueryable(), query);

        var descending = query.SortDirection == SortDirection.Descending;
        employees = ApplySorting(employees, query.SortBy, descending);

        return await employees.ToPagedListAsync(query.Page, query.PageSize);
    }

    public Task<List<EmployeeEquipmentEntity>> GetEquipmentAsync(int employeeId) =>
        Context.Set<EmployeeEquipmentEntity>()
            .AsNoTracking()
            .Include(assignment => assignment.Equipment)
            .ThenInclude(equipment => equipment.EquipmentType)
            .Where(equipment => equipment.EmployeeId == employeeId)
            .OrderBy(equipment => equipment.ReturnedOnUtc != null)
            .ThenByDescending(equipment => equipment.AssignedOnUtc)
            .ToListAsync();

    public async Task<int> AddEquipmentAsync(EmployeeEquipmentEntity equipment)
    {
        await Context.Set<EmployeeEquipmentEntity>().AddAsync(equipment);

        try
        {
            await Context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
                   { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_EmployeeEquipment_ActiveAssignment" })
        {
            throw new ConflictException("This asset is already assigned to an employee.");
        }

        return equipment.Id;
    }

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
