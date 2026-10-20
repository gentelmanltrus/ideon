using Ideon.API.Data;
using Ideon.API.DTOs.Comments;
using Ideon.API.Models;
using Ideon.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Services;
/// <summary>
/// Implements the ICommentService interface, providing methods for managing comments in the application.
public class CommentService : ICommentService
{
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CommentResponseDto>> GetByIdeaIdAsync(
        int ideaId)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.IdeaId == ideaId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                IdeaId = c.IdeaId,
                AuthorId = c.UserId,
                AuthorUsername = c.User.Username,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CommentResponseDto?> CreateAsync(
        int ideaId,
        int userId,
        CreateCommentDto dto)
    {
        var ideaExists = await _context.Ideas
            .AnyAsync(i => i.Id == ideaId);

        if (!ideaExists)
        {
            return null;
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            throw new ArgumentException("User does not exist.");
        }

        var content = dto.Content.Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment cannot be empty.");
        }

        var comment = new Comment
        {
            IdeaId = ideaId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.Id == comment.Id)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                IdeaId = c.IdeaId,
                AuthorId = c.UserId,
                AuthorUsername = c.User.Username,
                CreatedAt = c.CreatedAt
            })
            .FirstAsync();
    }
}