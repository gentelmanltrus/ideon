using Ideon.API.DTOs.Auth;

namespace Ideon.API.Services.Interfaces;
/// <summary>
/// Defines the contract for authentication-related operations.
public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}