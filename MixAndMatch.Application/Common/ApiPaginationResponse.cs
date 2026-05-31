namespace MixAndMatch.Application.Common;

public record PaginationMetadata(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrev
);

public record ApiPaginationResponse<T>(
    bool Success,
    string? Message,
    IEnumerable<T> Data,
    PaginationMetadata Metadata
)
{
    public static ApiPaginationResponse<T> Ok(
        IEnumerable<T> data, int totalCount, int page, int pageSize, string? message = null)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new(true, message, data, new PaginationMetadata(
            TotalCount: totalCount,
            Page:       page,
            PageSize:   pageSize,
            TotalPages: totalPages,
            HasNext:    page < totalPages,
            HasPrev:    page > 1
        ));
    }

    public static ApiPaginationResponse<T> Fail(string message) =>
        new(false, message, [], new PaginationMetadata(0, 0, 0, 0, false, false));
}
