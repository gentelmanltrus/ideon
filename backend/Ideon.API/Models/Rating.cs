namespace Ideon.API.Models;

public class Rating
{
    public int Id { get; set; }

    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public int Originality { get; set; }

    public int Feasibility { get; set; }

    public int Usefulness { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Idea Idea { get; set; } = null!;

    public User User { get; set; } = null!;
}