namespace Emplyx.Shared.Access;

public sealed record AccessStatusListResponse(
    IReadOnlyList<AccessStatusListItem> Items,
    int TotalCount);
