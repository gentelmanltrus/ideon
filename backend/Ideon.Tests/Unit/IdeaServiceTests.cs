using Ideon.API.Data;
using Ideon.API.DTOs.Ideas;
using Ideon.API.Models;
using Ideon.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Ideon.Tests.Unit;
/// Unit tests for the IdeaService class.
//this class contains unit tests for the IdeaService class, which is responsible for managing ideas in the application. The tests cover various scenarios, including retrieving all ideas, getting an idea by ID, creating, updating, and deleting ideas. The tests use an in-memory database to simulate the application's data context.
public class IdeaServiceTests
{
    private const int AuthorId = 100;
    private const int CategoryId = 100;
    //fact attribute indicates that the method is a test method that should be executed by the test runner.
    [Fact]
    public async Task GetAllAsync_ReturnsIdeasWithAuthorAndCategoryDetails()
    {   
        //await using statement ensures that the context is disposed of properly after use. The CreateContext method creates a new instance of the AppDbContext with an in-memory database for testing purposes.
        await using var context = CreateContext();
        await AddIdeaAsync(context);
        await AddIdeaAsync(context, id: 2, title: "Another idea");
        var service = new IdeaService(context);

        var ideas = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, ideas.Count);
        Assert.Contains(ideas, idea =>
            idea.Title == "A useful idea"
            && idea.AuthorUsername == "test-author"
            && idea.CategoryName == "Technology");
        Assert.Contains(ideas, idea => idea.Title == "Another idea");
    }
    //fact attribute indicates that the method is a test method that should be executed by the test runner.
    [Fact]
    public async Task GetByIdAsync_WhenIdeaExists_ReturnsIdeaDetails()
    {
        await using var context = CreateContext();
        await AddIdeaAsync(context);
        var service = new IdeaService(context);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("A useful idea", result.Title);
        Assert.Equal("test-author", result.AuthorUsername);
        Assert.Equal("Technology", result.CategoryName);
        Assert.Equal(AuthorId, result.AuthorId);
        Assert.Equal(CategoryId, result.CategoryId);
        Assert.True(result.LookingForBuilder);
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdeaDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        var service = new IdeaService(context);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_TrimsTextAndReturnsCreatedIdea()
    {
        await using var context = CreateContext();
        await AddAuthorAndCategoryAsync(context);
        var service = new IdeaService(context);
        var dto = new CreateIdeaDto
        {
            Title = "  New idea  ",
            Description = "  A detailed description  ",
            CategoryId = CategoryId,
            LookingForBuilder = true
        };

        var result = await service.CreateAsync(dto, AuthorId);

        Assert.Equal("New idea", result.Title);
        Assert.Equal("A detailed description", result.Description);
        Assert.Equal(AuthorId, result.AuthorId);
        Assert.Equal(CategoryId, result.CategoryId);
        Assert.Equal("test-author", result.AuthorUsername);
        Assert.Equal("Technology", result.CategoryName);
        Assert.True(result.LookingForBuilder);
        Assert.Single(await context.Ideas.ToListAsync());
    }

    [Fact]
    public async Task CreateAsync_WhenAuthorDoesNotExist_ThrowsArgumentException()
    {
        await using var context = CreateContext();
        await AddAuthorAndCategoryAsync(context);
        var service = new IdeaService(context);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(
                new CreateIdeaDto { Title = "Idea", Description = "Description", CategoryId = CategoryId },
                authorId: 999));
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryDoesNotExist_ThrowsArgumentException()
    {
        await using var context = CreateContext();
        await AddAuthorAndCategoryAsync(context);
        var service = new IdeaService(context);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(
                new CreateIdeaDto { Title = "Idea", Description = "Description", CategoryId = 999 },
                AuthorId));
    }

    [Fact]
    public async Task UpdateAsync_WhenIdeaExists_UpdatesAndReturnsIdea()
    {
        await using var context = CreateContext();
        await AddIdeaAsync(context);
        var service = new IdeaService(context);
        var dto = new UpdateIdeaDto
        {
            Title = "Updated idea",
            Description = "Updated description",
            CategoryId = CategoryId,
            LookingForBuilder = false
        };

        var result = await service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.CategoryId, result.CategoryId);
        Assert.Equal(dto.LookingForBuilder, result.LookingForBuilder);
        Assert.Equal("test-author", result.AuthorUsername);
    }

    [Fact]
    public async Task UpdateAsync_WhenIdeaDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        var service = new IdeaService(context);

        var result = await service.UpdateAsync(999, new UpdateIdeaDto
        {
            Title = "Updated idea",
            Description = "Updated description",
            CategoryId = CategoryId
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdeaExists_DeletesIdeaAndReturnsTrue()
    {
        await using var context = CreateContext();
        await AddIdeaAsync(context);
        var service = new IdeaService(context);

        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(await context.Ideas.ToListAsync());
    }

    [Fact]
    public async Task DeleteAsync_WhenIdeaDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext();
        var service = new IdeaService(context);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"IdeaServiceTests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    private static async Task AddIdeaAsync(
        AppDbContext context,
        int id = 1,
        string title = "A useful idea")
    {
        await AddAuthorAndCategoryAsync(context);
        context.Ideas.Add(new Idea
        {
            Id = id,
            Title = title,
            Description = "A detailed description",
            AuthorId = AuthorId,
            CategoryId = CategoryId,
            LookingForBuilder = true,
            Author = await context.Users.FindAsync(AuthorId) ?? throw new InvalidOperationException(),
            Category = await context.Categories.FindAsync(CategoryId) ?? throw new InvalidOperationException()
        });
        await context.SaveChangesAsync();
    }
    /// Adds an author and category to the database context if they do not already exist.
    private static async Task AddAuthorAndCategoryAsync(AppDbContext context)
    {
        if (!await context.Users.AnyAsync(user => user.Id == AuthorId))
        {
            context.Users.Add(new User
            {
                Id = AuthorId,
                Username = "test-author",
                Email = "test@example.com",
                PasswordHash = "test-hash"
            });
        }

        if (!await context.Categories.AnyAsync(category => category.Id == CategoryId))
        {
            context.Categories.Add(new Category
            {
                Id = CategoryId,
                Name = "Technology"
            });
        }

        await context.SaveChangesAsync();
    }
}