using System.ComponentModel.DataAnnotations;

namespace InfigoHR.Authentication.Models;

public sealed class SignUpRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset? StartedOn { get; set; }
}