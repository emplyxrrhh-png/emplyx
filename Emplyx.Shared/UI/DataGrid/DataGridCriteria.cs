namespace Emplyx.Shared.UI.DataGrid;

public sealed record DataGridCriteria(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    IReadOnlyDictionary<string, string?>? Filters = null);
