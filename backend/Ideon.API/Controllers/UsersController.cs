using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ideon.API.Data;
using Ideon.API.Models;
using Ideon.API.DTOs.Users;
using Ideon.API.DTOs.Ideas;

namespace Ideon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(string username)
        {
            var user = await _context.Users
                .Include(u => u.Ideas)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound(new { message = $"User '{username}' not found." });
            }

            var profile = new UserProfileDto
            {
                Username = user.Username,
                Bio = "Passionate builder and innovator in the Ideon community.",
                Ideas = user.Ideas
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new IdeaResponseDto
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        CategoryName = i.Category != null ? i.Category.Name : "General",
                        CreatedAt = i.CreatedAt,
                        LookingForBuilder = i.LookingForBuilder
                    })
                    .ToList()
            };

            return Ok(profile);
        }
    }
}