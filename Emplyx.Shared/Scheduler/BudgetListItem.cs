namespace Emplyx.Shared.Scheduler;

public sealed record BudgetListItem(
    string Id,
    string UnitName,
    string PeriodLabel,
    string Status,
    decimal PlannedHours,
    decimal ActualHours,
    DateTimeOffset UpdatedAt);
