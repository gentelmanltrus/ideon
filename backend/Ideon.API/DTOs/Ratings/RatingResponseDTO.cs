using System.ComponentModel.DataAnnotations;

namespace Ideon.API.DTOs.Ratings 
{
    public int Id { get; set; }

    public int IdeaId { get; set; }

    public int UserId { get; set; }

    public int Originality { get; set; }

    public int Feasibility { get; set; }

    public int Usefulness { get; set; }

    public DateTime CreatedAt { get; set; }
}
