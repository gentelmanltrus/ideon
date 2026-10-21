using System.Security.Claims;
using Ideon.API.DTOs.Ratings;
using Ideon.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
/// <summary> The RatingController class handles HTTP requests related to ratings, providing endpoints for retrieving, creating, and updating
[ApiController]
[Route("api/ideas/{ideaId:int}/ratings")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetAll(
        int ideaId)
    {
        return Ok(await _ratingService.GetByIdeaIdAsync(ideaId));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RatingResponseDto>> Create(
        int ideaId,
        CreateRatingDTO dto)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var rating = await _ratingService.CreateAsync(
                ideaId,
                userId,
                dto);

            if (rating is null)
            {
                return NotFound(new { message = "Idea not found." });
            }

            return Ok(rating);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("mine")]
    [Authorize]
    public async Task<ActionResult<RatingResponseDto>> UpdateMine(
        int ideaId,
        CreateRatingDTO dto)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var rating = await _ratingService.UpdateMineAsync(
            ideaId,
            userId,
            dto);

        if (rating is null)
        {
            return NotFound(new
            {
                message = "You have not rated this idea yet."
            });
        }

        return Ok(rating);
    }
}