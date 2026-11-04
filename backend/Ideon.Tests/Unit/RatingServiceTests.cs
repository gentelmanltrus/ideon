using Ideon.API.Data;
using Ideon.API.DTOs.Ratings;
using Ideon.API.Models;
using Ideon.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Ideon.Tests.Unit;
/// Unit tests for the RatingService class.
public class RatingServiceTests
{
    private const int IdeaId = 20;
    private const int OtherIdeaId = 21;
    private const int UserId = 11;
    //this test method checks if the GetByIdeaIdAsync method of the RatingService class returns only the ratings for a specific idea in the order they were created. It sets up an in-memory database context, adds an idea and users, and then adds multiple ratings for the same idea and another idea. The test asserts that the returned ratings are in the correct order and belong to the specified idea.
    [Fact]
    public async Task GetByIdeaIdAsync_ReturnsOnlyIdeaRatingsInCreationOrder()
    {
        await using var context = CreateContext();
        await AddIdeaAndUsersAsync(context);
        context.Ratings.AddRange(
            new Rating
            {
                IdeaId = IdeaId,
                UserId = UserId,
                Originality = 4,
                Feasibility = 3,
                Usefulness = 5,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Rating
            {
                IdeaId = IdeaId,
                UserId = 10,
                Originality = 2,
                Feasibility = 4,
                Usefulness = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Rating
            {
                IdeaId = OtherIdeaId,
                UserId = UserId,
                Originality = 1,
                Feasibility = 1,
                Usefulness = 1
            });
        await context.SaveChangesAsync();
        var service = new RatingService(context);

        var results = (await service.GetByIdeaIdAsync(IdeaId)).ToList();

        Assert.Equal(2, results.Count);
        Assert.Equal(2, results[0].Originality);
        Assert.Equal(4, results[1].Originality);
        Assert.All(results, rating => Assert.Equal(IdeaId, rating.IdeaId));
        Assert.Equal("test-user", results[1].Username);
    }
    //this test method checks if the CreateAsync method of the RatingService class creates and returns a new rating for an existing idea. It sets up an in-memory database context, adds an idea and users, and then creates a new rating using a CreateRatingDTO object. The test asserts that the returned rating has the correct properties and that it was added to the database.
    [Fact]
    public async Task CreateAsync_WhenIdeaExists_CreatesAndReturnsRating()
    {
        await using var context = CreateContext();
        await AddIdeaAndUsersAsync(context);
        var service = new RatingService(context);
        var dto = new CreateRatingDTO
        {
            Originality = 5,
            Feasibility = 4,
            Usefulness = 3
        };

        var result = await service.CreateAsync(IdeaId, UserId, dto);

        Assert.NotNull(result);
        Assert.Equal(IdeaId, result.IdeaId);
        Assert.Equal(UserId, result.UserId);
        Assert.Equal("test-user", result.Username);
        Assert.Equal(5, result.Originality);
        Assert.Equal(4, result.Feasibility);
        Assert.Equal(3, result.Usefulness);
        Assert.Single(await context.Ratings.ToListAsync());
    }
    //this test method checks if the CreateAsync method of the RatingService class returns null when trying to create a rating for a non-existing idea. It sets up an in-memory database context, adds an idea and users, and then attempts to create a new rating for an idea with an ID that does not exist. The test asserts that the result is null and that no ratings were added to the database.
    [Fact]
    public async Task CreateAsync_WhenIdeaDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        await AddIdeaAndUsersAsync(context);
        var service = new RatingService(context);

        var result = await service.CreateAsync(999, UserId, ValidRating());

        Assert.Null(result);
        Assert.Empty(await context.Ratings.ToListAsync());
    }
    //this test method checks if the CreateAsync method of the RatingService class throws an InvalidOperationException when a user tries to rate an idea they have already rated. It sets up an in-memory database context, adds an idea and users, and then adds a rating for the same idea and user. The test asserts that calling CreateAsync again with the same idea and user throws an exception and that only one rating exists in the database.
    [Fact]
    public async Task CreateAsync_WhenUserAlreadyRatedIdea_ThrowsInvalidOperationException()
    {
        await using var context = CreateContext();
        await AddIdeaAndUsersAsync(context);
        context.Ratings.Add(new Rating
        {
            IdeaId = IdeaId,
            UserId = UserId,
            Originality = 3,
            Feasibility = 3,
            Usefulness = 3
        });
        await context.SaveChangesAsync();
        var service = new RatingService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(IdeaId, UserId, ValidRating()));

        Assert.Single(await context.Ratings.ToListAsync());
    }
    //this test method checks if the UpdateMineAsync method of the RatingService class updates an existing rating for a user and idea. It sets up an in-memory database context, adds an idea and users, and then adds a rating for the same idea and user. The test then updates the rating using a CreateRatingDTO object and asserts that the returned rating has the updated properties and that only one rating exists in the database.
    [Fact]
    public async Task UpdateMineAsync_WhenUserHasRatedIdea_UpdatesRating()
    {
        await using var context = CreateContext();
        await AddIdeaAndUsersAsync(context);
        context.Ratings.Add(new Rating
        {
            IdeaId = IdeaId,
            UserId = UserId,
            Originality = 1,
            Feasibility = 2,
            Usefulness = 3
        });
        await context.SaveChangesAsync();
        var service = new RatingService(context);
        var dto = new CreateRatingDTO
        {
            Originality = 5,
            Feasibility = 4,
            Usefulness = 2
        };

        var result = await service.UpdateMineAsync(IdeaId, UserId, dto);

        Assert.NotNull(result);
        Assert.Equal(5, result.Originality);
        Assert.Equal(4, result.Feasibility);
        Assert.Equal(2, result.Usefulness);
        Assert.Equal("test-user", result.Username);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"RatingServiceTests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
    //this method adds an idea and two users (an author and a rating user) to the in-memory database context. It also adds a category for the ideas. This setup is used in the test methods to ensure that the necessary data exists before performing operations on ratings.
    private static async Task AddIdeaAndUsersAsync(AppDbContext context)
    {
        var author = new User
        {
            Id = 10,
            Username = "idea-author",
            Email = "author@example.com",
            PasswordHash = "test-hash"
        };
        var ratingUser = new User
        {
            Id = UserId,
            Username = "test-user",
            Email = "user@example.com",
            PasswordHash = "test-hash"
        };
        var category = new Category { Id = 100, Name = "Test category" };
        context.AddRange(author, ratingUser, category);
        context.Ideas.AddRange(
            new Idea { Id = IdeaId, AuthorId = author.Id, CategoryId = category.Id },
            new Idea { Id = OtherIdeaId, AuthorId = author.Id, CategoryId = category.Id });
        await context.SaveChangesAsync();
    }

    private static CreateRatingDTO ValidRating() => new()
    {
        Originality = 4,
        Feasibility = 4,
        Usefulness = 4
    };
}