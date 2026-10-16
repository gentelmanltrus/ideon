using Microsoft.AspNetCore.Mvc;
using Ideon.API.Interfaces;
using Ideon.API.Models;
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
	public async Task<ActionResult<IReadOnlyCollection<Rating>>> GetAll(CancellationToken cancellationToken)
	{
		return Ok(await _ratingService.GetRatingsAsync());
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<Rating>> GetById(int id, CancellationToken cancellationToken)
	{
		var ratings = await _ratingService.GetRatingsAsync();
		var rating = ratings.FirstOrDefault(rating => rating.Id == id);
		return rating is null ? NotFound() : Ok(rating);
	}

	[HttpPost]
	public async Task<ActionResult<Rating>> Create(
		[FromBody] Rating request,
		CancellationToken cancellationToken)
	{
		var rating = await _ratingService.CreateRatingAsync(request);
		return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(
		int id,
		[FromBody] Rating request,
		CancellationToken cancellationToken)
	{
		return await _ratingService.UpdateRatingAsync(id, request)
			? NoContent()
			: NotFound();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		return await _ratingService.DeleteRatingAsync(id)
			? NoContent()
			: NotFound();
	}
}