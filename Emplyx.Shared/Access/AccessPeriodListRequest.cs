using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Access;

public sealed record AccessPeriodListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? WeekDay = null,
    string? Month = null,
    string? Status = null,
    bool? IsSpecial = null)
{
    public static AccessPeriodListRequest FromCriteria(
        DataGridCriteria criteria,
        string? weekDay = null,
        string? month = null,
        string? status = null,
        bool? isSpecial = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            weekDay,
            month,
            status,
            isSpecial);
}
