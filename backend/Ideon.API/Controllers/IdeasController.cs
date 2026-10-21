using Ideon.API.DTOs.Ideas;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace Ideon.API.Controllers;
// <summary>
// The IdeasController class handles HTTP requests related to ideas, providing endpoints for CRUD operations.
[ApiController]
[Route("api/[controller]")]
public class IdeasController : ControllerBase
{
    private readonly IIdeaService _ideaService;

    public IdeasController(IIdeaService ideaService)
    {
        _ideaService = ideaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdeaResponseDto>>> GetAll()
    {
        var ideas = await _ideaService.GetAllAsync();

        return Ok(ideas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IdeaResponseDto>> GetById(int id)
    {
        var idea = await _ideaService.GetByIdAsync(id);

        if (idea is null)
        {
            return NotFound();
        }

        return Ok(idea);
    }
    // method handles the creation of a new idea. It retrieves the user ID from the claims of the authenticated user, calls the CreateAsync method of the IIdeaService to create the idea, and returns an appropriate response based on the result. If the creation is successful, it returns a CreatedAtAction response with the created idea; if there is an error, it returns a BadRequest response with an error message.
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<IdeaResponseDto>> Create(
    CreateIdeaDto dto)
    {
    var userIdValue = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Unauthorized();
    }

    try
    {
        var idea = await _ideaService.CreateAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = idea.Id },
            idea);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
// method handles the updating of an existing idea. It retrieves the user ID from the claims of the authenticated user, checks if the idea exists and if the authenticated user is the author of the idea, and then calls the UpdateAsync method of the IIdeaService to update the idea. It returns an appropriate response based on the result, such as Ok with the updated idea, NotFound if the idea does not exist, or Forbid if the user is not authorized to update the idea.
[HttpPut("{id:int}")]
[Authorize]
public async Task<ActionResult<IdeaResponseDto>> Update(
    int id,
    UpdateIdeaDto dto)
{
    var userIdValue = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Unauthorized();
    }

    var existingIdea = await _ideaService.GetByIdAsync(id);

    if (existingIdea is null)
    {
        return NotFound();
    }

    if (existingIdea.AuthorId != userId)
    {
        return Forbid();
    }

    var updatedIdea = await _ideaService.UpdateAsync(id, dto);

    return Ok(updatedIdea);
}
// method handles the deletion of an existing idea. It retrieves the user ID from the claims of the authenticated user, checks if the idea exists and if the authenticated user is the author of the idea, and then calls the DeleteAsync method of the IIdeaService to delete the idea. It returns an appropriate response based on the result, such as NoContent for successful deletion, NotFound if the idea does not exist, or Forbid if the user is not authorized to delete the idea.
[HttpDelete("{id:int}")]
[Authorize]
public async Task<IActionResult> Delete(int id)
{
    var userIdValue = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Unauthorized();
    }

    var existingIdea = await _ideaService.GetByIdAsync(id);

    if (existingIdea is null)
    {
        return NotFound();
    }

    if (existingIdea.AuthorId != userId)
    {
        return Forbid();
    }

    var deleted = await _ideaService.DeleteAsync(id);

    return deleted ? NoContent() : NotFound();
}
}