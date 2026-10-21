using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Auth;
/// Represents the data transfer object (DTO) for user login.
public class LoginDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}