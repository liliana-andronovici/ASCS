using System.ComponentModel.DataAnnotations;

namespace InfigoHR.Authentication.Models;

public sealed class SignInRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}