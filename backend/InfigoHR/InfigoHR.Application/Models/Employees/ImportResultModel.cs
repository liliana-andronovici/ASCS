namespace InfigoHR.Application.Models.Employees;

public class ImportResultModel
{
    public int Imported { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}
