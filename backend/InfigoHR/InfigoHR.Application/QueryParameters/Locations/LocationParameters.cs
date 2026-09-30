namespace InfigoHR.Application.QueryParameters.Locations;

using InfigoHR.Application.Common;

public class LocationParameters : Parameters
{
    public string? Search { get; set; }
    public LocationSortBy? SortBy { get; set; }
}
