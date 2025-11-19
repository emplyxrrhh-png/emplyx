namespace Emplyx.Shared.Scheduler;

public sealed record DailyMoveListItem(
    string Id,
    string EmployeeName,
    string Terminal,
    DateTimeOffset Time,
    string Type,
    string Source,
    string Status,
    bool RequiresAttention);
