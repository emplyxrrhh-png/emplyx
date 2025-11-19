namespace Emplyx.Shared.Access;

public sealed record AccessPeriodListResponse(
    IReadOnlyList<AccessPeriodListItem> Items,
    int TotalCount);
