namespace Ideon.API.Models;

public class CollaborationRequest
{
    public int Id { get; set; }

    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Idea Idea { get; set; } = null!;

    public User User { get; set; } = null!;
}