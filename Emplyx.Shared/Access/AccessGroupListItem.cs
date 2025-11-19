namespace Emplyx.Shared.Access;

public sealed record AccessGroupListItem(
    string Id,
    string Name,
    string? Description,
    string Status,
    int ZonesCount,
    int PeriodsCount,
    int EmployeesCount,
    DateTimeOffset UpdatedAt,
    bool HasPendingChanges);
