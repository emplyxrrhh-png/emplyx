namespace Emplyx.Shared.Contracts.Modulos;

public sealed record ModuloDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    bool EsCritico);

public sealed record UpsertModuloRequest(
    Guid? Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    bool EsCritico);
