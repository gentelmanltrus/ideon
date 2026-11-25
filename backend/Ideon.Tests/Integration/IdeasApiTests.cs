using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ideon.Tests.Integration;
//this class contains integration tests for the Ideas API endpoints. It uses a WebApplicationFactory to create an in-memory test server and an HttpClient to send requests to the API. The tests verify that the API returns the expected status codes and content types, as well as validating the structure of the JSON responses.
public sealed class IdeasApiTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;
    //the constructor takes a WebApplicationFactory<Program> as a parameter, which is used to create an HttpClient for sending requests to the API. The factory is provided by the IClassFixture interface, which ensures that the same instance of the factory is used for all tests in this class.
	public IdeasApiTests(WebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}
    //this test method sends a GET request to the /api/ideas endpoint and verifies that the response has a status code of 200 OK, a content type of application/json, and that the response body is a JSON array. The test uses the HttpClient to send the request and the JsonDocument class to parse and validate the JSON response.
	[Fact]
	public async Task GetIdeas_ReturnsOkAndJsonArray()
	{
		using var response = await _client.GetAsync("/api/ideas");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        //the response content is read as a stream and parsed into a JsonDocument. The test then checks that the root element of the JSON document is an array, indicating that the API returned a list of ideas.
		await using var content = await response.Content.ReadAsStreamAsync();
		using var document = await JsonDocument.ParseAsync(content);
		Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
	}
}
