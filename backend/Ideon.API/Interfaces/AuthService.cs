using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ideon.API.Data;
using Ideon.API.DTOs.Auth;
using Ideon.API.Models;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ideon.API.Services;
/// Implements the IAuthService interface, providing methods for user registration and login.
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher = new();
    /// Initializes a new instance of the AuthService class with the specified AppDbContext and IConfiguration.
    public AuthService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    // Registers a new user based on the provided RegisterDto object. If the username or email already exists, it returns null. Otherwise, it creates a new user, hashes the password, saves the user to the database, and returns an AuthResponseDto containing the authentication token and user information.
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var username = dto.Username.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());

        if (usernameExists)
        {
            return null;
        }

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email);

        if (emailExists)
        {
            return null;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
        //password hashing - The user's password is hashed using the PasswordHasher<User> class before being stored in the database. This ensures that the password is not stored in plain text, enhancing security.
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreateAuthResponse(user);
    }
    // Logs in a user based on the provided LoginDto object. It checks if the username or email exists, verifies the password, and returns an AuthResponseDto containing the authentication token and user information if successful. If the user does not exist or the password is incorrect, it returns null.
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var identifier = dto.UsernameOrEmail.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == identifier ||
            u.Email.ToLower() == identifier);

        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateAuthResponse(user);
    }
    // Creates an AuthResponseDto containing the authentication token and user information for the specified user. It generates a JWT token with claims for the user's ID, username, and email, and sets the token's expiration time to 8 hours from the current time.
    private AuthResponseDto CreateAuthResponse(User user)
    {
        var keyText = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is missing.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is missing.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyText));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(ClaimTypes.Name, user.Username),

            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}