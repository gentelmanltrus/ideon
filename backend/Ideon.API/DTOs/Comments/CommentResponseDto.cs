namespace Ideon.API.DTOs.Comments;

// <summary>
// Represents the data transfer object (DTO) for responding with comment details.
public class CommentResponseDto
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int IdeaId { get; set; }

    public int AuthorId { get; set; }

    public string AuthorUsername { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}