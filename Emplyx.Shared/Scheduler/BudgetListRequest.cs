using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Scheduler;

public sealed record BudgetListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Unit = null,
    string? Status = null,
    string? Period = null)
{
    public static BudgetListRequest FromCriteria(
        DataGridCriteria criteria,
        string? unit = null,
        string? status = null,
        string? period = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            unit,
            status,
            period);
}
