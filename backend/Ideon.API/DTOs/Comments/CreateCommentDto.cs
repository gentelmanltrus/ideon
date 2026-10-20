using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Comments;

// <summary>
// Represents the data transfer object (DTO) for creating a new comment.
public class CreateCommentDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}