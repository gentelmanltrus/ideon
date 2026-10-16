
using Ideon.API.Data;
using Ideon.API.DTOs.Comments;
using Ideon.API.Models;
using Ideon.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Services;
/// <summary>
/// Implements the comment service for managing comment-related operations.
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
        CreateCommentDto dto)
    {
        var ideaExists = await _context.Ideas
            .AnyAsync(i => i.Id == ideaId);

        if (!ideaExists)
        {
            return null;
        }

        var authorExists = await _context.Users
            .AnyAsync(u => u.Id == dto.AuthorId);

        if (!authorExists)
        {
            throw new ArgumentException("Author does not exist.");
        }

        var comment = new Comment
        {
            IdeaId = ideaId,
            UserId = dto.AuthorId,
            Content = dto.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return await _context.Comments
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
