using Ideon.API.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Ideon.API.Extensions;
//this static class provides an extension method for IQueryable<T> to facilitate pagination of query results. The ToPagedResultAsync method takes a query, page number, and page size as parameters, and returns a PagedResult<T> containing the paginated items along with pagination metadata such as total count and total pages.
public static class PaginationExtensions
{   

    //this method performs pagination on an IQueryable<T> query. It calculates the total count of items, retrieves the specified page of items, and returns a PagedResult<T> containing the items and pagination information. It also includes validation for the page number and page size, throwing exceptions if they are out of range
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page));
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(
            items,
            page,
            pageSize,
            totalCount
        );
    }
}