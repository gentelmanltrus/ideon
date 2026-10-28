using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.CollaborationRequests;
//this DTO is used to update the status of a collaboration request. It contains a single property, Status, which is required and must be either "Accepted" or "Rejected". The RegularExpression attribute is used to enforce this validation rule.
public class UpdateCollaborationRequestStatusDto
{
    [Required]
    [RegularExpression(
        "^(Accepted|Rejected)$",
        ErrorMessage = "Status must be Accepted or Rejected.")]
    public string Status { get; set; } = string.Empty;
}