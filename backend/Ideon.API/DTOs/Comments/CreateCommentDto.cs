using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Comments;

// <summary>
// Represents the data transfer object (DTO) for creating a new comment.
public class CreateCommentDto
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int IdeaId { get; set; }

    [Required]
    public int AuthorId { get; set; }
}