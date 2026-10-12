using InfigoHR.Application.Models.Employees;

namespace InfigoHR.Application.Interfaces.Services;

public interface IOrgChartService
{
    Task<IEnumerable<OrgChartEmployeeModel>> GetOrgChartAsync();
}
