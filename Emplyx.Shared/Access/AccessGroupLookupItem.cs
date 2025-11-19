namespace Emplyx.Shared.Access;

public sealed record AccessGroupLookupItem(
    string Id,
    string Name,
    string? Zone = null,
    string? Period = null,
    string Status = "Activo");
