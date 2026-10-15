using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace Ideon.API.Controllers;
// <summary>
// The CommentsController class handles HTTP requests related to comments, providing endpoints for CRUD operations.
[ApiController]
[Route("api/[controller]")]
public sealed class CommentsController : ControllerBase
{
	private static readonly ConcurrentDictionary<int, Comment> Comments = new();
	private static int _nextId;

	[HttpGet]
	public ActionResult<IEnumerable<Comment>> GetAll()
	{
		return Ok(Comments.Values.OrderBy(comment => comment.Id));
	}

	[HttpGet("{id:int}")]
	public ActionResult<Comment> GetById(int id)
	{
		return Comments.TryGetValue(id, out var comment) ? Ok(comment) : NotFound();
	}

	[HttpPost]
	public ActionResult<Comment> Create([FromBody] CommentInput input)
	{
		if (string.IsNullOrWhiteSpace(input.Content))
		{
			return BadRequest("Comment content is required.");
		}

		var comment = new Comment
		{
			Id = Interlocked.Increment(ref _nextId),
			Content = input.Content.Trim(),
			PostId = input.PostId,
			UserId = input.UserId
		};

		Comments[comment.Id] = comment;
		return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
	}

	[HttpPut("{id:int}")]
	public IActionResult Update(int id, [FromBody] CommentInput input)
	{
		if (string.IsNullOrWhiteSpace(input.Content))
		{
			return BadRequest("Comment content is required.");
		}

		if (!Comments.TryGetValue(id, out var existing))
		{
			return NotFound();
		}

		var updated = new Comment
		{
			Id = id,
			Content = input.Content.Trim(),
			PostId = input.PostId,
			UserId = input.UserId
		};

		return Comments.TryUpdate(id, updated, existing) ? NoContent() : Conflict();
	}

	[HttpDelete("{id:int}")]
	public IActionResult Delete(int id)
	{
		return Comments.TryRemove(id, out _) ? NoContent() : NotFound();
	}
}

public sealed class Comment
{
	public int Id { get; set; }
	public string Content { get; set; } = string.Empty;
	public int PostId { get; set; }
	public int UserId { get; set; }
}

public sealed class CommentInput
{
	public string Content { get; set; } = string.Empty;
	public int PostId { get; set; }
	public int UserId { get; set; }
}
