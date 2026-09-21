namespace InfigoHR.Presentation.Models;

public class ValidationErrorResponseModel
{
    public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
}
