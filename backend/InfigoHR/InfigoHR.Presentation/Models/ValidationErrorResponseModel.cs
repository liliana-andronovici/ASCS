namespace InfigoHR.Presentation.Models;

public sealed class ValidationErrorResponseModel
{
    public required IEnumerable<string> Errors { get; set; }
}