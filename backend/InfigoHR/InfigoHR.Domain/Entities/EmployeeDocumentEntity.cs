using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class EmployeeDocumentEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}