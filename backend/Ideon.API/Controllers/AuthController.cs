using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ideon.API.Data;
using Ideon.API.Models;

namespace Ideon.API.Controllers;

// <summary>
// The AuthController class handles HTTP requests related to user authentication and registration.

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // Registers a new user account
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Username and password are required." });
        }

        // Check if username is already taken
        var existingUser = await _context.Users.AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower().Trim());
        if (existingUser)
        {
            return BadRequest(new { message = "Username is already taken." });
        }

        // Create a new user
        var newUser = new User
        {
            Username = dto.Username.Trim(),
            PasswordHash = dto.Password, // Storing in plaintext for now
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = newUser.Id,
            username = newUser.Username
        });
    }

    // Authenticates an existing user
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Username and password are required." });
        }

        // Look up user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower().Trim());

        if (user == null || user.PasswordHash != dto.Password)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username
        });
    }
}

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}