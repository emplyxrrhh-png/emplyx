namespace Emplyx.Shared.Access;

public sealed record AccessPeriodLookupItem(
    string Id,
    string Name,
    string WeekDayLabel,
    string RangeLabel,
    string Status);
