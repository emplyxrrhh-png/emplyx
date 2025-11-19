namespace Emplyx.Shared.Access;

public sealed record AccessEventListItem(
    string Id,
    string Name,
    string ZoneName,
    string GroupName,
    DateTimeOffset ScheduledDate,
    string Status,
    bool AllowsCopy,
    bool AllowsDelete);
