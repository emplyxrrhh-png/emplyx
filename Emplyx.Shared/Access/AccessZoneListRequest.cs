using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Access;

public sealed record AccessZoneListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Type = null,
    string? Parent = null,
    bool? IsCritical = null)
{
    public static AccessZoneListRequest FromCriteria(
        DataGridCriteria criteria,
        string? type = null,
        string? parent = null,
        bool? isCritical = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            type,
            parent,
            isCritical);
}
