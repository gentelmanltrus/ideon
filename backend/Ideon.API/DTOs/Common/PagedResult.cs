namespace Ideon.API.DTOs.Common;

//this generic record represents a paginated result set, containing a list of items of type T, along with pagination information such as the current page, page size, total count of items, total pages, and flags indicating whether there are previous or next pages.
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount
)
{   

    // Calculates the total number of pages based on the total count of items and the page size. It uses Math.Ceiling to round up to the nearest whole number, ensuring that any remaining items are accounted for in an additional page.
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);
// Indicates whether there is a previous page available based on the current page number. If the current page is greater than 1, it returns true; otherwise, it returns false.
    public bool HasPreviousPage => Page > 1;
    // Indicates whether there is a next page available based on the current page number and the total number of pages. If the current page is less than the total number of pages, it returns true; otherwise, it returns false.
    public bool HasNextPage => Page < TotalPages;
}