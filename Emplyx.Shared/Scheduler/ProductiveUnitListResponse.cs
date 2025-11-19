namespace Emplyx.Shared.Scheduler;

public sealed record ProductiveUnitListResponse(
    IReadOnlyList<ProductiveUnitListItem> Items,
    int TotalCount);
