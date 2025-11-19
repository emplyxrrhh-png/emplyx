namespace Emplyx.Shared.Scheduler;

public sealed record ProductiveUnitListItem(
    string Id,
    string Name,
    string Plant,
    string Manager,
    string Status,
    int Budgets,
    int Employees,
    DateTimeOffset UpdatedAt);
