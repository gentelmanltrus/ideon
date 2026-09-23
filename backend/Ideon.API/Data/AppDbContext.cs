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

}