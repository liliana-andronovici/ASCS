namespace InfigoHR.Application.Models.Departments;

// changed: use model + one model per file
public class DepartmentModel
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}
