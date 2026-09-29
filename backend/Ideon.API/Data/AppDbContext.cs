using Ideon.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Data;
// <summary>
// AppDbContext is the main class for interacting with the database.
// </summary>
public class AppDbContext : DbContext
{   
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for configuring the context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    /// Gets or sets the Classes DbSet.
    public DbSet<User> Users => Set<User>();
    public DbSet<Idea> Ideas => Set<Idea>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<CollaborationRequest> CollaborationRequests => Set<CollaborationRequest>();

    /// <summary>
    ///constraints and relationships between the entities in the model. also seeds the database with initial data for the Category entity.
    /// </summary>
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // here you can configure your entity relationships, constraints, etc.
        // User constraints
         modelBuilder.Entity<User>()
        .HasIndex(u => u.Username)
        .IsUnique();
        //Email constraints
        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
        // Category constraints
        modelBuilder.Entity<Category>()
        .HasIndex(c => c.Name)
        .IsUnique();
        // Rating constraints
        modelBuilder.Entity<Rating>()
        .HasIndex(r => new { r.UserId, r.IdeaId })
        .IsUnique();
        //Rating constraints for Originality, Feasibility and Usefulness to be between 1 and 5
        modelBuilder.Entity<Rating>()
        .ToTable("Ratings", table =>
        {
            table.HasCheckConstraint(
            "CK_Ratings_Originality",
            "\"Originality\" BETWEEN 1 AND 5");

             table.HasCheckConstraint(
            "CK_Ratings_Feasibility",
            "\"Feasibility\" BETWEEN 1 AND 5");

        table.HasCheckConstraint(
            "CK_Ratings_Usefulness",
            "\"Usefulness\" BETWEEN 1 AND 5");
    });
    // Seed initial data for Categories
    modelBuilder.Entity<Category>().HasData(
    new Category
    {
        Id = 1,
        Name = "Game Development"
    },
    new Category
    {
        Id = 2,
        Name = "Technology"
    },
    new Category
    {
        Id = 3,
        Name = "Education"
    },
    new Category
    {
        Id = 4,
        Name = "Entertainment"
    },
    new Category
    {
        Id = 5,
        Name = "Social"
    },
    new Category
    {
        Id = 6,
        Name = "Other"
    }
);
}
}