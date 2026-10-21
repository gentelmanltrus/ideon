using Ideon.API.DTOs.Ratings;

namespace Ideon.API.Interfaces;
/// <summary>
/// Defines the contract for rating-related operations.
public interface IRatingService
{
    Task<IEnumerable<RatingResponseDto>> GetByIdeaIdAsync(int ideaId);

    Task<RatingResponseDto?> CreateAsync(
        int ideaId,
        int userId,
        CreateRatingDTO dto);

    Task<RatingResponseDto?> UpdateMineAsync(
        int ideaId,
        int userId,
        CreateRatingDTO dto);
}