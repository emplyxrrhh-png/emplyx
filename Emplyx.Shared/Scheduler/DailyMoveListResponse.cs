namespace Emplyx.Shared.Scheduler;

public sealed record DailyMoveListResponse(
    IReadOnlyList<DailyMoveListItem> Items,
    int TotalCount);
