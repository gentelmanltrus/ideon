using Ideon.API.Data;
using Ideon.API.DTOs.CollaborationRequests;
using Ideon.API.Models;
using Ideon.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Services;
/// <summary> Implements the ICollaborationRequestService interface, providing methods for managing collaboration requests in the application.
public class CollaborationRequestService : ICollaborationRequestService
{
    private readonly AppDbContext _context;

    public CollaborationRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(
        CollaborationRequestResponseDto? Request,
        string? ErrorCode
    )> CreateAsync(int ideaId, int userId)
    {
        var idea = await _context.Ideas
            .FirstOrDefaultAsync(i => i.Id == ideaId);

        if (idea is null)
        {
            return (null, "idea_not_found");
        }

        if (!idea.LookingForBuilder)
        {
            return (null, "not_looking_for_builder");
        }

        if (idea.AuthorId == userId)
        {
            return (null, "own_idea");
        }

        var alreadyRequested = await _context.CollaborationRequests
            .AnyAsync(r =>
                r.IdeaId == ideaId &&
                r.UserId == userId);

        if (alreadyRequested)
        {
            return (null, "already_requested");
        }

        var request = new CollaborationRequest
        {
            IdeaId = ideaId,
            UserId = userId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.CollaborationRequests.Add(request);
        await _context.SaveChangesAsync();

        var response = await _context.CollaborationRequests
            .AsNoTracking()
            .Where(r => r.Id == request.Id)
            .Select(r => new CollaborationRequestResponseDto
            {
                Id = r.Id,
                IdeaId = r.IdeaId,
                IdeaTitle = r.Idea.Title,
                UserId = r.UserId,
                Username = r.User.Username,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .FirstAsync();

        return (response, null);
    }

    public async Task<IEnumerable<CollaborationRequestResponseDto>>
        GetMineAsync(int userId)
    {
        return await _context.CollaborationRequests
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CollaborationRequestResponseDto
            {
                Id = r.Id,
                IdeaId = r.IdeaId,
                IdeaTitle = r.Idea.Title,
                UserId = r.UserId,
                Username = r.User.Username,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }
}