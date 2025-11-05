namespace Emplyx.Shared.Contracts.Permisos;

public sealed record PermisoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    Guid ModuloId,
    string? Categoria,
    bool EsCritico);

public sealed record UpsertPermisoRequest(
    Guid? Id,
    Guid ModuloId,
    string Codigo,
    string Nombre,
    string? Categoria,
    bool EsCritico);
