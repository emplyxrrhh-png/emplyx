namespace Emplyx.Shared.Access;

public sealed record AccessStatusListItem(
    string Id,
    string GroupName,
    string ZoneName,
    string EmployeeName,
    string StatusName,
    string Severity,
    DateTimeOffset LastEvent,
    bool RequiresAttention);
