using Ideon.API.Models;
// <summary>
// The IRatingService interface defines the contract for managing ratings, including methods for retrieving, creating, updating, and deleting ratings.
// </summary>
namespace Ideon.API.Interfaces
{
	public interface IRatingService
	{
		Task<IEnumerable<Rating>> GetRatingsAsync();
		Task<Rating?> GetRatingByIdAsync(int id);
		Task<Rating> CreateRatingAsync(Rating rating);
		Task<bool> UpdateRatingAsync(int id, Rating rating);
		Task<bool> DeleteRatingAsync(int id);
	}
}
