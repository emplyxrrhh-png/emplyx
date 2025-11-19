using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Scheduler;

public sealed record DailyMoveListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Employee = null,
    string? Terminal = null,
    string? Type = null,
    string? Status = null,
    DateOnly? Day = null)
{
    public static DailyMoveListRequest FromCriteria(
        DataGridCriteria criteria,
        string? employee = null,
        string? terminal = null,
        string? type = null,
        string? status = null,
        DateOnly? day = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            employee,
            terminal,
            type,
            status,
            day);
}
