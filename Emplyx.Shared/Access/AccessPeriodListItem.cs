namespace Emplyx.Shared.Access;

public sealed record AccessPeriodListItem(
    string Id,
    string Name,
    string Status,
    string? Description,
    string WeekDayLabel,
    string FromTime,
    string ToTime,
    string? MonthLabel,
    DateTimeOffset UpdatedAt,
    bool IsSpecialPeriod);
