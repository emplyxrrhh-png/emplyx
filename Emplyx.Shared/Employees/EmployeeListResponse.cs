namespace Emplyx.Shared.Employees;

public sealed record EmployeeListResponse(
    IReadOnlyList<EmployeeListItem> Items,
    int TotalCount);
