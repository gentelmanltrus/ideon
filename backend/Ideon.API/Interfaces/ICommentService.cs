
using Ideon.API.DTOs.Comments;

namespace Ideon.API.Services.Interfaces;
/// <summary>
/// Defines the contract for comment-related operations.
/// </summary>
public interface ICommentService
{
    Task<IEnumerable<CommentResponseDto>> GetByIdeaIdAsync(int ideaId);

    Task<CommentResponseDto?> CreateAsync(
        int ideaId,
        CreateCommentDto dto);
}
