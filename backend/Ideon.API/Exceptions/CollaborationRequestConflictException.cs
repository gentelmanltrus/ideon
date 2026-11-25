namespace Ideon.API.Exceptions;
// this exception is thrown when a collaboration request conflicts with an existing request in the database.
public class CollaborationRequestConflictException : Exception
{
    public CollaborationRequestConflictException(string message)
        : base(message)
    {
    }
}