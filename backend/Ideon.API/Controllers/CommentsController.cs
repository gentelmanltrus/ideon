
using Ideon.API.DTOs.Comments;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
/// <summary>
/// Controller for managing comments related to ideas.
/// </summary>
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
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetAll(
        int ideaId)
    {
        var comments =
            await _commentService.GetByIdeaIdAsync(ideaId);

        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> Create(
        int ideaId,
        CreateCommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            return BadRequest("Comment content cannot be empty.");
        }

        if (dto.Content.Length > 1000)
        {
            return BadRequest("Comment cannot exceed 1000 characters.");
        }

        try
        {
            var comment =
                await _commentService.CreateAsync(ideaId, dto);

            if (comment is null)
            {
                return NotFound("Idea not found.");
            }

            return Created($"/api/ideas/{ideaId}/comments/{comment.Id}",
                comment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
