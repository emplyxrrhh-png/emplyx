using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Access;

public sealed record AccessGroupListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Zone = null,
    string? Period = null,
    string? Status = null,
    bool? HasPendingChanges = null)
{
    public static AccessGroupListRequest FromCriteria(
        DataGridCriteria criteria,
        string? zone = null,
        string? period = null,
        string? status = null,
        bool? hasPendingChanges = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            zone,
            period,
            status,
            hasPendingChanges);
}
