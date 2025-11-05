namespace Emplyx.Shared.Contracts.Roles;

public sealed record RolDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    bool IsSystem,
    Guid? ClearanceId,
    IReadOnlyCollection<Guid> Permisos,
    IReadOnlyCollection<Guid> Delegaciones);

public sealed record UpsertRolRequest(
    Guid? Id,
    string Nombre,
    string? Descripcion,
    bool IsSystem,
    Guid? ClearanceId,
    IReadOnlyCollection<Guid> Permisos);
