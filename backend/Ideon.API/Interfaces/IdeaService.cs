using Ideon.API.Data;
using Ideon.API.DTOs.Common;
using Ideon.API.DTOs.Ideas;
using Ideon.API.Extensions;
using Ideon.API.Models;
using Ideon.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Ideon.API.Services;

// <summary>
// Implements the IIdeaService interface, providing methods for managing ideas in the application.
public class IdeaService : IIdeaService
{
    private readonly AppDbContext _context;

    public IdeaService(AppDbContext context)
    {
        _context = context; // context - The AppDbContext instance is injected into the IdeaService constructor, allowing it to interact with the database.
    }
    // Retrieves a paginated list of ideas from the database based on the specified page number and page size. It returns a PagedResult<IdeaResponseDto> containing the paginated items along with pagination metadata.
    public async Task<PagedResult<IdeaResponseDto>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default,
        int? categoryId = null,
        string? search = null,
        string sortBy = "newest")
    {
        var query = _context.Ideas
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        var normalizedSearch = search?.Trim().ToLower();
        if (!string.IsNullOrEmpty(normalizedSearch))
        {
            query = query.Where(i =>
                i.Title.ToLower().Contains(normalizedSearch) ||
                i.Description.ToLower().Contains(normalizedSearch) ||
                i.Author.Username.ToLower().Contains(normalizedSearch) ||
                i.Category.Name.ToLower().Contains(normalizedSearch));
        }

        query = sortBy switch
        {
            "oldest" => query.OrderBy(i => i.CreatedAt).ThenBy(i => i.Id),
            "title" => query.OrderBy(i => i.Title).ThenBy(i => i.Id),
            "newest" => query.OrderByDescending(i => i.CreatedAt).ThenByDescending(i => i.Id),
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), "Unsupported sort order.")
        };

        var projectedQuery = query.Select(i => new IdeaResponseDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            AuthorId = i.AuthorId,
            AuthorUsername = i.Author.Username,
            CategoryId = i.CategoryId,
            CategoryName = i.Category.Name,
            LookingForBuilder = i.LookingForBuilder,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            RatingCount = i.Ratings.Count,
            AverageOriginality = i.Ratings.Any() ? i.Ratings.Average(r => r.Originality) : 0,
            AverageFeasibility = i.Ratings.Any() ? i.Ratings.Average(r => r.Feasibility) : 0,
            AverageUsefulness = i.Ratings.Any() ? i.Ratings.Average(r => r.Usefulness) : 0
        });

        return await projectedQuery.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    // Retrieves all ideas from the database and returns them as a collection of IdeaResponseDto objects
    public async Task<IEnumerable<IdeaResponseDto>> GetAllAsync()
    {
        return await _context.Ideas
            .Include(i => i.Author)
            .Include(i => i.Category)
            .Include(i => i.Ratings)
            .Select(i => new IdeaResponseDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                AuthorId = i.AuthorId,
                AuthorUsername = i.Author.Username,
                CategoryId = i.CategoryId,
                CategoryName = i.Category.Name,
                LookingForBuilder = i.LookingForBuilder,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                RatingCount = i.Ratings.Count,
                AverageOriginality = i.Ratings.Any() ? i.Ratings.Average(r => r.Originality) : 0,
                AverageFeasibility = i.Ratings.Any() ? i.Ratings.Average(r => r.Feasibility) : 0,
                AverageUsefulness = i.Ratings.Any() ? i.Ratings.Average(r => r.Usefulness) : 0
            })
            .ToListAsync(); //convert to dto - The retrieved ideas are projected into IdeaResponseDto objects, which are then returned as a list.
    }
    // Retrieves a specific idea by its ID from the database and returns it as an IdeaResponseDto object. If the idea is not found, it returns null.
    public async Task<IdeaResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Ideas
        .Include(i => i.Author)
        .Include(i => i.Category)
        .Where(i => i.Id == id)
        .Select(i => new IdeaResponseDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            AuthorId = i.AuthorId,
            AuthorUsername = i.Author.Username,
            CategoryId = i.CategoryId,
            CategoryName = i.Category.Name,
            LookingForBuilder = i.LookingForBuilder,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        })
        .FirstOrDefaultAsync();
    }

    // Creates a new idea in the database based on the provided CreateIdeaDto object and returns the created idea as an IdeaResponseDto object.
    public async Task<IdeaResponseDto> CreateAsync(
    CreateIdeaDto dto,
    int authorId)
    {
        var authorExists = await _context.Users
            .AnyAsync(u => u.Id == authorId);

        if (!authorExists)
        {
            throw new ArgumentException("Authenticated user does not exist.");
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            throw new ArgumentException("Category does not exist.");
        }

        var idea = new Idea
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            AuthorId = authorId,
            CategoryId = dto.CategoryId,
            LookingForBuilder = dto.LookingForBuilder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Ideas.Add(idea);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(idea.Id))!;
    }
    // Updates an existing idea in the database based on the provided ID and UpdateIdeaDto object. If the idea is found, it updates its properties and returns the updated idea as an IdeaResponseDto object. If the idea is not found, it returns null.
    public async Task<IdeaResponseDto?> UpdateAsync(
    int id,
    UpdateIdeaDto dto)
    {
        var idea = await _context.Ideas.FindAsync(id);

        if (idea is null)
        {
            return null;
        }

        idea.Title = dto.Title;
        idea.Description = dto.Description;
        idea.CategoryId = dto.CategoryId;
        idea.LookingForBuilder = dto.LookingForBuilder;
        idea.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(id);
    }

    // Deletes an existing idea from the database based on the provided ID. If the idea is found and deleted, it returns true; otherwise, it returns false.
    public async Task<bool> DeleteAsync(int id)
    {
        var idea = await _context.Ideas.FindAsync(id);

        if (idea is null)
        {
            return false;
        }

        _context.Ideas.Remove(idea);
        await _context.SaveChangesAsync();

        return true;
    }
}