namespace Ideon.API.DTOs.Ideas;

/// <summary>
/// Represents the data transfer object (DTO) for responding with idea details.
public class IdeaResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    public string AuthorUsername { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public bool LookingForBuilder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public double AverageOriginality { get; set; }

    public double AverageFeasibility { get; set; }

    public double AverageUsefulness { get; set; }

    public int RatingCount { get; set; }
}