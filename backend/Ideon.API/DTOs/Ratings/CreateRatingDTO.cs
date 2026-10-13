
using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Ratings;

public class CreateRatingDTO
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Originality { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Feasibility { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Usefulness { get; set; }
}
