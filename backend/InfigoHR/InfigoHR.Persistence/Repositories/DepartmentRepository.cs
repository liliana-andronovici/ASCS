using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InfigoHR.Application.Common;
using InfigoHR.Domain.Common;
using InfigoHR.Persistence.Extensions;
using InfigoHR.Application.QueryParameters.Departments;

namespace InfigoHR.Persistence.Repositories;

public class DepartmentRepository : GenericRepository<DepartmentEntity>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<DepartmentEntity>> SearchAsync(DepartmentParameters paginationParams)
    {
        var query = Entities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(paginationParams.Search))
        {
            query = query.Where(d =>
                EF.Functions.ILike(d.Name, $"%{paginationParams.Search}%"));
        }

        query = ApplySorting(query, paginationParams);

        return await query.ToPagedListAsync(
            paginationParams.Page,
            paginationParams.PageSize);
    }

    private IQueryable<DepartmentEntity> ApplySorting(
        IQueryable<DepartmentEntity> query,
        DepartmentParameters paginationParams)
    {
        return paginationParams.SortBy switch
        {
            DepartmentSortBy.Name => paginationParams.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(d => d.Name)
                : query.OrderBy(d => d.Name),

            _ => paginationParams.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(d => d.Id)
                : query.OrderBy(d => d.Id)
        };
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await Entities.AnyAsync(x => x.Id == id);
    }
}