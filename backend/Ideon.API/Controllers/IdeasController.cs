using Ideon.API.DTOs.Ideas;
using Ideon.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<ActionResult<IdeaResponseDto>> Create(
        CreateIdeaDto dto)
    {
        var idea = await _ideaService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = idea.Id },
            idea);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IdeaResponseDto>> Update(
        int id,
        UpdateIdeaDto dto)
    {
        var idea = await _ideaService.UpdateAsync(id, dto);

        if (idea is null)
        {
            return NotFound();
        }

        return Ok(idea);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _ideaService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}