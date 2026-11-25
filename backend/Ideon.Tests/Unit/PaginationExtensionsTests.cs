using System.Collections.Generic;
using Ideon.API.DTOs.Common;
using Ideon.API.Extensions;
using Xunit;

namespace Ideon.Tests.Unit;

public class PaginationExtensionsTests
{
	[Fact]
	public void PagedResult_CalculatesTotalPages()
	{
		var result = new PagedResult<string>(
			new List<string> { "A", "B" },
			Page: 1,
			PageSize: 2,
			TotalCount: 5);

		Assert.Equal(3, result.TotalPages);
		Assert.True(result.HasNextPage);
		Assert.False(result.HasPreviousPage);
	}

	[Fact]
	public void PagedResult_LastPageHasNoNextPage()
	{
		var result = new PagedResult<string>(
			new List<string> { "E" },
			Page: 3,
			PageSize: 2,
			TotalCount: 5);

		Assert.Equal(3, result.TotalPages);
		Assert.False(result.HasNextPage);
		Assert.True(result.HasPreviousPage);
	}

	[Fact]
	public void PagedResult_ZeroItemsHasNoPages()
	{
		var result = new PagedResult<string>(
			new List<string>(),
			Page: 1,
			PageSize: 2,
			TotalCount: 0);

		Assert.Equal(0, result.TotalPages);
		Assert.False(result.HasNextPage);
		Assert.False(result.HasPreviousPage);
	}
}
