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
    //this method retrieves all incoming collaboration requests for a specific author. It takes the author's ID as a parameter and returns a collection of CollaborationRequestResponseDto objects representing the incoming requests.
    Task<IEnumerable<CollaborationRequestResponseDto>>
    GetIncomingAsync(int authorId);
    //this method retrieves all outgoing collaboration requests for a specific user. It takes the user's ID as a parameter and returns a collection of CollaborationRequestResponseDto objects representing the outgoing requests.
    Task<(
    CollaborationRequestResponseDto? Request,
    string? ErrorCode
)>
    UpdateStatusAsync(
        int requestId,
        int authorId,
        string status);
}