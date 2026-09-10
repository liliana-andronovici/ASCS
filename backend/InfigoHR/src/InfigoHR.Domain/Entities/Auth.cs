namespace InfigoHR.Domain.Entities;

// Parola NU se ține niciodată în plaintext, doar hash-ul (ex: BCrypt/Argon2).

public class Auth
{
    public int EmployeeId { get; set; } // PK, FK
    public string PasswordHash { get; set; } = string.Empty;

    // Forgot-password flow
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}