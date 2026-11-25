namespace Ideon.API.Exceptions;
// this exception is thrown when an idea with a specific ID is not found in the database.
public class IdeaNotFoundException : Exception
{
    public IdeaNotFoundException(int ideaId)
        : base($"Idea with ID {ideaId} was not found.")
    {
        IdeaId = ideaId;
    }

    public int IdeaId { get; }
}