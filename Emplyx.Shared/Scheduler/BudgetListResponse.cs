namespace Emplyx.Shared.Scheduler;

public sealed record BudgetListResponse(
    IReadOnlyList<BudgetListItem> Items,
    int TotalCount);
