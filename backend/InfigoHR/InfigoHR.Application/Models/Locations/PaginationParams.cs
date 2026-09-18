namespace InfigoHR.Application.Models.Locations;
using InfigoHR.Application.Common;

public class PaginationParams : QueryParameters
{
    public LocationSortBy? SortBy { get; set; }
}