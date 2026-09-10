namespace InfigoHR.Domain.Entities;

public class TwoFactorAuth
{
    public int EmployeeId { get; set; } // PK, FK
    public string SecretKey { get; set; } = string.Empty;

    public virtual Employee Employee { get; set; } = null!;
}
