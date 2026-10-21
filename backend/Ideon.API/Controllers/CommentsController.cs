using System.Security.Claims;
using Ideon.API.DTOs.Comments;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
/// <summary>
/// The CommentsController class handles HTTP requests related to comments, providing endpoints for retrieving and creating comments
[ApiController]
[Route("api/ideas/{ideaId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetAll(
        int ideaId)
    {
        var comments =
            await _commentService.GetByIdeaIdAsync(ideaId);

        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentResponseDto>> Create(
        int ideaId,
        CreateCommentDto dto)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            return BadRequest(new
            {
                message = "Comment content cannot be empty."
            });
        }

        try
        {
            var comment = await _commentService.CreateAsync(
                ideaId,
                userId,
                dto);

            if (comment is null)
            {
                return NotFound(new
                {
                    message = "Idea not found."
                });
            }

            return Created(
                $"/api/ideas/{ideaId}/comments",
                comment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}