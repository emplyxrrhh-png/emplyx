namespace Emplyx.Shared.Access;

public sealed record AccessEventListResponse(
    IReadOnlyList<AccessEventListItem> Items,
    int TotalCount);
