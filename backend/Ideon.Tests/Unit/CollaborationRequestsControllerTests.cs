using System.Security.Claims;
using Ideon.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Ideon.Tests.Unit;
//this class contains unit tests for the CollaborationRequestsController, specifically testing the behavior of the controller methods when the user claim (NameIdentifier) is missing or invalid. The tests verify that the controller returns UnauthorizedResult in these scenarios, ensuring proper authentication handling.
public class CollaborationRequestsControllerTests
{
    //this test verifies that the Create method of the CollaborationRequestsController returns an UnauthorizedResult when the NameIdentifier claim is missing from the user's claims. It creates a controller instance without any user claims and calls the Create method, asserting that the result is of type UnauthorizedResult.
	[Fact]
	public async Task Create_ReturnsUnauthorized_WhenNameIdentifierIsMissing()
	{
		var controller = CreateController();

		var result = await controller.Create(42);

		Assert.IsType<UnauthorizedResult>(result);
	}
    //this test checks that the Create method of the CollaborationRequestsController returns an UnauthorizedResult when the NameIdentifier claim is present but contains an invalid value (not a number). It creates a controller instance with an invalid user claim and calls the Create method, asserting that the result is of type UnauthorizedResult.
	[Fact]
	public async Task GetMine_ReturnsUnauthorized_WhenNameIdentifierIsInvalid()
	{
		var controller = CreateController("not-a-number");

		var result = await controller.GetMine();

		Assert.IsType<UnauthorizedResult>(result);
	}
    //this test verifies that the GetIncoming method of the CollaborationRequestsController returns an UnauthorizedResult when the NameIdentifier claim is missing from the user's claims. It creates a controller instance without any user claims and calls the GetIncoming method, asserting that the result is of type UnauthorizedResult.
	[Fact]
	public async Task GetIncoming_ReturnsUnauthorized_WhenNameIdentifierIsMissing()
	{
		var controller = CreateController();

		var result = await controller.GetIncoming();

		Assert.IsType<UnauthorizedResult>(result);
	}
    //this test checks that the UpdateStatus method of the CollaborationRequestsController returns an UnauthorizedResult when the NameIdentifier claim is present but contains an invalid value (not a number). It creates a controller instance with an invalid user claim and calls the UpdateStatus method, asserting that the result is of type UnauthorizedResult.
	[Fact]
	public async Task UpdateStatus_ReturnsUnauthorized_WhenNameIdentifierIsInvalid()
	{
		var controller = CreateController("invalid");

		var result = await controller.UpdateStatus(7, null!);

		Assert.IsType<UnauthorizedResult>(result);
	}
    //this private helper method creates an instance of the CollaborationRequestsController with a specified user claim (NameIdentifier). It sets up the controller's HttpContext with the provided user claim, allowing tests to simulate different authentication scenarios. If no userId is provided, it creates a controller with no claims.
	private static CollaborationRequestsController CreateController(string? userId = null)
	{
		// The service is never reached in these tests because the user claim is invalid.
		var controller = new CollaborationRequestsController(null!);
		var claims = userId is null
			? Array.Empty<Claim>()
			: new[] { new Claim(ClaimTypes.NameIdentifier, userId) };

		controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
			}
		};

		return controller;
	}
}
