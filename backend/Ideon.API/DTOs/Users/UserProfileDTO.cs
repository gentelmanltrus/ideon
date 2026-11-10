using Ideon.API.DTOs.Ideas;
namespace Ideon.API.DTOs.Users;

public class UserProfileDto
{
    public string Username { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public List<IdeaResponseDto> Ideas { get; set; } = new();
}