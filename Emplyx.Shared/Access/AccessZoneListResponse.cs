namespace Emplyx.Shared.Access;

public sealed record AccessZoneListResponse(
    IReadOnlyList<AccessZoneListItem> Items,
    int TotalCount);
