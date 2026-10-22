namespace InfigoHR.Application.Models.Employees;

public class EmployeeAccess
{
    public int EmployeeId { get; }
    public bool CanManageEmployees { get; }

    public EmployeeAccess(int employeeId, bool canManageEmployees)
    {
        EmployeeId = employeeId;
        CanManageEmployees = canManageEmployees;
    }
}