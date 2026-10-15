using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
// <summary>
// Controller for managing ratings.
[ApiController]
[Route("api/[controller]")]
public sealed class RatingController : ControllerBase
{
	private readonly IRatingService _ratingService;

	public RatingController(IRatingService ratingService)
	{
		_ratingService = ratingService;
	}

	[HttpGet]
	public async Task<ActionResult<IReadOnlyCollection<RatingDto>>> GetAll(CancellationToken cancellationToken)
	{
		return Ok(await _ratingService.GetAllAsync(cancellationToken));
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<RatingDto>> GetById(int id, CancellationToken cancellationToken)
	{
		var rating = await _ratingService.GetByIdAsync(id, cancellationToken);
		return rating is null ? NotFound() : Ok(rating);
	}

	[HttpPost]
	public async Task<ActionResult<RatingDto>> Create(
		[FromBody] CreateRatingRequest request,
		CancellationToken cancellationToken)
	{
		if (request.Score is < 1 or > 5)
			return BadRequest("Score must be between 1 and 5.");

		var rating = await _ratingService.CreateAsync(request, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(
		int id,
		[FromBody] UpdateRatingRequest request,
		CancellationToken cancellationToken)
	{
		if (request.Score is < 1 or > 5)
			return BadRequest("Score must be between 1 and 5.");

		return await _ratingService.UpdateAsync(id, request, cancellationToken)
			? NoContent()
			: NotFound();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		return await _ratingService.DeleteAsync(id, cancellationToken)
			? NoContent()
			: NotFound();
	}
}

public interface IRatingService
{
	Task<IReadOnlyCollection<RatingDto>> GetAllAsync(CancellationToken cancellationToken);
	Task<RatingDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
	Task<RatingDto> CreateAsync(CreateRatingRequest request, CancellationToken cancellationToken);
	Task<bool> UpdateAsync(int id, UpdateRatingRequest request, CancellationToken cancellationToken);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public sealed record RatingDto(int Id, int UserId, int TargetId, int Score, string? Comment);
public sealed record CreateRatingRequest(int UserId, int TargetId, int Score, string? Comment);
public sealed record UpdateRatingRequest(int UserId, int TargetId, int Score, string? Comment);
