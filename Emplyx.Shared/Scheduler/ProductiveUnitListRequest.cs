using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Scheduler;

public sealed record ProductiveUnitListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Plant = null,
    string? Status = null,
    string? Manager = null)
{
    public static ProductiveUnitListRequest FromCriteria(
        DataGridCriteria criteria,
        string? plant = null,
        string? status = null,
        string? manager = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            plant,
            status,
            manager);
}
