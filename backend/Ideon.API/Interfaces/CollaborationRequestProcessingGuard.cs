using System.Collections.Concurrent;

namespace Ideon.API.Services;
// This class is used to prevent concurrent processing of the same collaboration request. It maintains a thread-safe collection of request IDs that are currently being processed. When a request is being processed, its ID is added to the collection, and when processing is complete, the ID is removed. This ensures that only one thread can process a specific request at a time.
public class CollaborationRequestProcessingGuard
{
    private readonly ConcurrentDictionary<int, byte> _processing = new(); // A thread-safe collection to track currently processing request IDs.

    // This method attempts to enter the processing state for a specific request ID. If the request ID is not already being processed, it adds the ID to the collection and returns true. If the request ID is already being processed, it returns false.
    public bool TryEnter(int requestId)
    {
        return _processing.TryAdd(requestId, 0);
    }

    // This method removes a request ID from the processing collection, indicating that the request is no longer being processed.
    public void Exit(int requestId)
    {
        _processing.TryRemove(requestId, out _);
    }
}