using Ideon.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Interfaces;
// <summary>
// The RatingService class implements the IRatingService interface, providing methods for managing ratings in the database, including retrieving, creating, updating, and deleting ratings.
// </summary>
public class RatingService : IRatingService
{
	private readonly DbContext _context;
	private readonly DbSet<Rating> _ratings;

	public RatingService(DbContext context)
	{
		_context = context;
		_ratings = context.Set<Rating>();
	}

	public async Task<IEnumerable<Rating>> GetRatingsAsync()
	{
		return await _ratings.AsNoTracking().ToListAsync();
	}

	public async Task<Rating?> GetRatingByIdAsync(int id)
	{
		return await _ratings.AsNoTracking().FirstOrDefaultAsync(rating => rating.Id == id);
	}

	public async Task<Rating> CreateRatingAsync(Rating rating)
	{
		ArgumentNullException.ThrowIfNull(rating);

		await _ratings.AddAsync(rating);
		await _context.SaveChangesAsync();
		return rating;
	}

	public async Task<bool> UpdateRatingAsync(int id, Rating rating)
	{
		ArgumentNullException.ThrowIfNull(rating);

		var existingRating = await _ratings.FindAsync(id);
		if (existingRating is null)
		{
			return false;
		}

		_context.Entry(existingRating).CurrentValues.SetValues(rating);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> DeleteRatingAsync(int id)
	{
		var rating = await _ratings.FindAsync(id);
		if (rating is null)
		{
			return false;
		}

		_ratings.Remove(rating);
		await _context.SaveChangesAsync();
		return true;
	}
}
