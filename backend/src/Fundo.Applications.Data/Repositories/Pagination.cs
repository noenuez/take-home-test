namespace Fundo.Applications.Data.Repositories;

public sealed record PageRequest(int PageNumber = 1, int PageSize = 10)
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int NormalizedPageNumber => PageNumber < 1 ? DefaultPageNumber : PageNumber;

    public int NormalizedPageSize
    {
        get
        {
            var normalized = PageSize < 1 ? DefaultPageSize : PageSize;
            return normalized > MaxPageSize ? MaxPageSize : normalized;
        }
    }
}

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
