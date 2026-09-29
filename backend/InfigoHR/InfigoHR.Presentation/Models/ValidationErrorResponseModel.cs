namespace InfigoHR.Presentation.Models;

public class ValidationErrorResponseModel
{
    public required IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
}