using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Ideas;
// <summary>
// Represents the data transfer object (DTO) for updating an existing idea.
public class UpdateIdeaDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public bool LookingForBuilder { get; set; }
}