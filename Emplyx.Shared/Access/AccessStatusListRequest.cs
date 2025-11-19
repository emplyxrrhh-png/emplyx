using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Access;

public sealed record AccessStatusListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Group = null,
    string? Zone = null,
    string? Severity = null,
    bool? RequiresAttention = null)
{
    public static AccessStatusListRequest FromCriteria(
        DataGridCriteria criteria,
        string? group = null,
        string? zone = null,
        string? severity = null,
        bool? requiresAttention = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            group,
            zone,
            severity,
            requiresAttention);
}
