using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Shared.Employees;

public sealed record EmployeeListRequest(
    int PageIndex,
    int PageSize,
    string? SortField = null,
    bool SortDescending = false,
    string? Search = null,
    string? Status = null,
    string? Group = null,
    string? Module = null,
    bool? HasForgottenRight = null)
{
    public static EmployeeListRequest FromCriteria(
        DataGridCriteria criteria,
        string? status = null,
        string? group = null,
        string? module = null,
        bool? hasForgottenRight = null)
        => new(
            criteria.PageIndex,
            criteria.PageSize,
            criteria.SortField,
            criteria.SortDescending,
            criteria.Search,
            status,
            group,
            module,
            hasForgottenRight);
}
