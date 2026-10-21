using System.Security.Claims;
using Ideon.API.DTOs.Auth;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
/// <summary>
/// Controller responsible for handling authentication-related endpoints.
[ApiController]
[Route("api/auth")]
//this class is responsible for handling authentication-related endpoints, such as user registration, login, and retrieving the current user's information. It uses the IAuthService to perform the actual authentication logic.
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result is null)
        {
            return Conflict(new
            {
                message = "Username or email is already registered."
            });
        }

        return Ok(result);
    }
    //this method handles the login endpoint. It accepts a LoginDto object containing the user's credentials, calls the LoginAsync method of the IAuthService, and returns an appropriate response based on the result. If the login is successful, it returns an AuthResponseDto with the authentication token and user information; otherwise, it returns an Unauthorized response with an error message.
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid username/email or password."
            });
        }

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var username = User.FindFirstValue(
            ClaimTypes.Name);

        var email = User.FindFirstValue(
            ClaimTypes.Email);

        return Ok(new
        {
            userId,
            username,
            email
        });
    }
}