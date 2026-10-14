namespace Ideon.API.DTOs.Ratings;

public class RatingResponseDto
{
    public int Id { get; set; }

    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public int Originality { get; set; }

    public int Feasibility { get; set; }

    public int Usefulness { get; set; }

    public DateTime CreatedAt { get; set; }
}