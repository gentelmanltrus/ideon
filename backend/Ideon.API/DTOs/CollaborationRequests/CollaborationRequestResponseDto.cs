namespace Ideon.API.DTOs.CollaborationRequests;
/// Represents the data transfer object (DTO) for collaboration request responses.
public class CollaborationRequestResponseDto
{
    public int Id { get; set; }

    public int IdeaId { get; set; }

    public string IdeaTitle { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}