namespace InfigoHR.Application.Models.Employees;

public class EmployeeProfileModel : EmployeeModel
{
    public string? Phone { get; set; }
    public string? Desk { get; set; }
    public int? BirthdayMonth { get; set; }
    public int? BirthdayDay { get; set; }
    public string[] Languages { get; set; } = [];
    public string? EmployeeCode { get; set; }
    public string? EmploymentType { get; set; }
    public string? ContractType { get; set; }
    public decimal? WeeklyHours { get; set; }
    public long ProfileVersion { get; set; }
    public List<EmployeeMainDataModel> DirectReports { get; set; } = [];
}