namespace InfigoHR.Application.Exceptions;

// changed: custom exception added
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
