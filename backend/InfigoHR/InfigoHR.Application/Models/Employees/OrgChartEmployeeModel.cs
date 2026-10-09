namespace InfigoHR.Application.Models.Employees;

public class OrgChartEmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Title { get; set; }

    public List<OrgChartEmployeeModel> DirectReports { get; set; } = [];
}