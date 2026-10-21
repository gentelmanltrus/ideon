using Ideon.API.Data;
using Ideon.API.DTOs.Ratings;
using Ideon.API.Interfaces;
using Ideon.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Services;
/// <summary> Implements the IRatingService interface, providing methods for managing ratings in the application.
public class RatingService : IRatingService
{
    private readonly AppDbContext _context;

    public RatingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RatingResponseDto>> GetByIdeaIdAsync(
        int ideaId)
    {
        return await _context.Ratings
            .AsNoTracking()
            .Where(r => r.IdeaId == ideaId)
            .OrderBy(r => r.CreatedAt)
            .Select(r => new RatingResponseDto
            {
                Id = r.Id,
                IdeaId = r.IdeaId,
                UserId = r.UserId,
                Username = r.User.Username,
                Originality = r.Originality,
                Feasibility = r.Feasibility,
                Usefulness = r.Usefulness,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<RatingResponseDto?> CreateAsync(
        int ideaId,
        int userId,
        CreateRatingDTO dto)
    {
        var ideaExists = await _context.Ideas
            .AnyAsync(i => i.Id == ideaId);

        if (!ideaExists)
        {
            return null;
        }

        var existing = await _context.Ratings
            .AnyAsync(r => r.IdeaId == ideaId && r.UserId == userId);

        if (existing)
        {
            throw new InvalidOperationException(
                "You have already rated this idea. Update your rating instead.");
        }

        var rating = new Rating
        {
            IdeaId = ideaId,
            UserId = userId,
            Originality = dto.Originality,
            Feasibility = dto.Feasibility,
            Usefulness = dto.Usefulness,
            CreatedAt = DateTime.UtcNow
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return await _context.Ratings
            .AsNoTracking()
            .Where(r => r.Id == rating.Id)
            .Select(r => new RatingResponseDto
            {
                Id = r.Id,
                IdeaId = r.IdeaId,
                UserId = r.UserId,
                Username = r.User.Username,
                Originality = r.Originality,
                Feasibility = r.Feasibility,
                Usefulness = r.Usefulness,
                CreatedAt = r.CreatedAt
            })
            .FirstAsync();
    }

    public async Task<RatingResponseDto?> UpdateMineAsync(
        int ideaId,
        int userId,
        CreateRatingDTO dto)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r =>
                r.IdeaId == ideaId && r.UserId == userId);

        if (rating is null)
        {
            return null;
        }

        rating.Originality = dto.Originality;
        rating.Feasibility = dto.Feasibility;
        rating.Usefulness = dto.Usefulness;

        await _context.SaveChangesAsync();

        return await _context.Ratings
            .AsNoTracking()
            .Where(r => r.Id == rating.Id)
            .Select(r => new RatingResponseDto
            {
                Id = r.Id,
                IdeaId = r.IdeaId,
                UserId = r.UserId,
                Username = r.User.Username,
                Originality = r.Originality,
                Feasibility = r.Feasibility,
                Usefulness = r.Usefulness,
                CreatedAt = r.CreatedAt
            })
            .FirstAsync();
    }
}