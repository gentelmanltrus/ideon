using System.Collections.Concurrent;

namespace Ideon.API.Services;
// Prevents concurrent processing of the same collaboration request by threads in this application process.
// The dictionary is shared memory within the process: callers using this guard instance see the same entries.
// It is not shared across separate processes, containers, or machines, so it cannot coordinate multiple
// service instances; that requires a distributed lock or another shared coordination store.
public class CollaborationRequestProcessingGuard
{
    private readonly ConcurrentDictionary<int, byte> _processing = new();

    // TryAdd is atomic: only the first thread to add this request ID succeeds.
    public bool TryEnter(int requestId)
    {
        return _processing.TryAdd(requestId, 0);
    }

    // Call when processing finishes so a later thread can enter for this request ID.
    public void Exit(int requestId)
    {
        _processing.TryRemove(requestId, out _);
    }
}