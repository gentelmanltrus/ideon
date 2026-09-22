namespace Ideon.API.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Idea> Ideas { get; set; } = new List<Idea>();
}