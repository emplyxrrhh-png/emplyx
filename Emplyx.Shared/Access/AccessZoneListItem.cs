namespace Emplyx.Shared.Access;

public sealed record AccessZoneListItem(
    string Id,
    string Name,
    string Type,
    string ParentName,
    string? Description,
    int DoorsCount,
    int SensorsCount,
    bool IsCritical,
    DateTimeOffset UpdatedAt);
