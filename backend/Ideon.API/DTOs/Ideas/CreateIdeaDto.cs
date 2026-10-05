using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Ideas;

// <summary>
// Represents the data transfer object (DTO) for creating a new idea.   
public class CreateIdeaDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    //Required - The Description property is marked as required, meaning that it must have a value when creating a new idea. If the Description is not provided, the validation will fail, and an error will be returned to the client.
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool LookingForBuilder { get; set; }
}