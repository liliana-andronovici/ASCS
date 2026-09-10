namespace InfigoHR.Domain.Entities;

// Relație 1-N cu Employee, "trusts". Un angajat poate avea mai multe device-uri de încredere.

public class TrustedDevice
{
    public string DeviceToken { get; set; } = string.Empty; // PK
    public int EmployeeId { get; set; } // FK
    public DateTime TrustedUntil { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
