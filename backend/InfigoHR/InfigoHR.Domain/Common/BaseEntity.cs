namespace InfigoHR.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }
}