using Ideon.API.DTOs.CollaborationRequests;

namespace Ideon.API.Services.Interfaces;

public interface ICollaborationRequestService
{
    Task<(
        CollaborationRequestResponseDto? Request,
        string? ErrorCode
    )> CreateAsync(int ideaId, int userId);

    Task<IEnumerable<CollaborationRequestResponseDto>>
        GetMineAsync(int userId);
}