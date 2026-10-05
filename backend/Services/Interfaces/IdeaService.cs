using Ideon.API.Data;
using Ideon.API.DTOs.Ideas;
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

    // Retrieves all ideas from the database and returns them as a collection of IdeaResponseDto objects
    public async Task<IEnumerable<IdeaResponseDto>> GetAllAsync()
    {
        return await _context.Ideas
            .Include(i => i.Author)
            .Include(i => i.Category)
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
    public async Task<IdeaResponseDto> CreateAsync(CreateIdeaDto dto)
{
    var idea = new Idea
    {
        Title = dto.Title,
        Description = dto.Description,
        AuthorId = dto.AuthorId,
        CategoryId = dto.CategoryId,
        LookingForBuilder = dto.LookingForBuilder,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    _context.Ideas.Add(idea); // The new idea is added to the Ideas DbSet in the AppDbContext, marking it for insertion into the database.

    await _context.SaveChangesAsync();// The changes are saved to the database - tells entity framework to persist the changes made to the context, including the newly added idea, to the underlying database. 

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