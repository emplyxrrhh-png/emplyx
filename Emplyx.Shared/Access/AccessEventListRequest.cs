using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Access;

public sealed record AccessEventListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Zone = null,
    string? Group = null,
    string? Status = null)
{
    public static AccessEventListRequest FromCriteria(
        DataGridCriteria criteria,
        string? zone = null,
        string? group = null,
        string? status = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            zone,
            group,
            status);
}
