namespace InfigoHR.Application.QueryParameters.Locations;

using InfigoHR.Application.Common;

public class PaginationParams : Parameters
{
    public string? Search { get; set; }
    public LocationSortBy? SortBy { get; set; }
}
