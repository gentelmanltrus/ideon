using System.Security.Claims;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
/// <summary> The CollaborationRequestsController class handles HTTP requests related to collaboration requests, providing endpoints for creating and retrieving collaboration requests for ideas.
[ApiController]
[Authorize]
[Route("api")]
public class CollaborationRequestsController : ControllerBase
{
    private readonly ICollaborationRequestService _service;

    public CollaborationRequestsController(
        ICollaborationRequestService service)
    {
        _service = service;
    }

    [HttpPost("ideas/{ideaId:int}/collaboration-requests")]
    public async Task<IActionResult> Create(int ideaId)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateAsync(ideaId, userId);

        if (result.Request is not null)
        {
            return Created(
                $"/api/collaboration-requests/mine",
                result.Request);
        }

        return result.ErrorCode switch
        {
            "idea_not_found" => NotFound(new
            {
                code = result.ErrorCode,
                message = "Idea not found."
            }),

            "not_looking_for_builder" => BadRequest(new
            {
                code = result.ErrorCode,
                message = "This idea is not looking for a builder."
            }),

            "own_idea" => BadRequest(new
            {
                code = result.ErrorCode,
                message = "You cannot request to collaborate on your own idea."
            }),

            "already_requested" => Conflict(new
            {
                code = result.ErrorCode,
                message = "You have already requested to collaborate on this idea."
            }),

            _ => StatusCode(500, new
            {
                message = "An unexpected error occurred."
            })
        };
    }

    [HttpGet("collaboration-requests/mine")]
    public async Task<IActionResult> GetMine()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var requests = await _service.GetMineAsync(userId);

        return Ok(requests);
    }
}