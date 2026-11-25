using Ideon.API.Services;
using Xunit;

namespace Ideon.Tests.Unit;
/// Unit tests for the CollaborationRequestProcessingGuard class.
public class CollaborationRequestProcessingGuardTests
{   
    //fact attribute indicates that the method is a test method that should be executed by the test runner.
	[Fact]
    // Test that TryEnter returns true when the request is not already being processed.
	public void TryEnter_ReturnsFalse_WhenRequestIsAlreadyProcessing()
	{
		var guard = new CollaborationRequestProcessingGuard();
        // Simulate a request being processed by calling TryEnter with a specific request ID.
		var first = guard.TryEnter(10);
		var second = guard.TryEnter(10);
        // Assert that the first call to TryEnter returns true (request can be processed) and the second call returns false (request is already being processed).
		Assert.True(first);
		Assert.False(second);
	}

	[Fact]
    // Test that Exit allows a request to be processed again after it has been marked as processing.
	public void Exit_AllowsRequestToBeProcessedAgain()
	{
		var guard = new CollaborationRequestProcessingGuard();
        // Simulate a request being processed by calling TryEnter with a specific request ID, then mark it as completed by calling Exit.
		guard.TryEnter(10);
		guard.Exit(10);

		var result = guard.TryEnter(10);

		Assert.True(result);
	}
}
