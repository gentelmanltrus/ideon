namespace Ideon.API.Models;

public class Idea
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    public int CategoryId { get; set; }

    public bool LookingForBuilder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User Author { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<CollaborationRequest> CollaborationRequests { get; set; }
        = new List<CollaborationRequest>();
}