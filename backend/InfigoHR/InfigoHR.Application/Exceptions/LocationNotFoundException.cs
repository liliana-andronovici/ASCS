namespace InfigoHR.Application.Exceptions;

public class LocationNotFoundException : Exception
{
    public LocationNotFoundException(int id) 
        : base($"Location with id {id} was not found.")
    {
    }
}