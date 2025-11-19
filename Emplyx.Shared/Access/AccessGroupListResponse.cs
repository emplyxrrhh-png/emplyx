namespace Emplyx.Shared.Access;

public sealed record AccessGroupListResponse(
    IReadOnlyList<AccessGroupListItem> Items,
    int TotalCount);
