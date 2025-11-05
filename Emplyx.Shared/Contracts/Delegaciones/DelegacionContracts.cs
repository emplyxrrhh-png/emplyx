namespace Emplyx.Shared.Contracts.Delegaciones;

public sealed record DelegacionDto(
    Guid Id,
    string Nombre,
    string? Codigo,
    string? Descripcion,
    Guid? ParentId,
    bool IsActive,
    IReadOnlyCollection<Guid> Roles);

public sealed record UpsertDelegacionRequest(
    Guid? Id,
    string Nombre,
    string? Codigo,
    string? Descripcion,
    Guid? ParentId,
    bool IsActive,
    IReadOnlyCollection<Guid> Roles);

public sealed record DelegacionTemporalDto(
    Guid Id,
    Guid DeleganteId,
    Guid DelegadoId,
    DateTime InicioUtc,
    DateTime FinUtc,
    bool AplicaTodosLosRoles,
    bool AprobadaMfa,
    string? MetodoMfa,
    string Estado,
    IReadOnlyCollection<Guid> Roles);

public sealed record CreateDelegacionTemporalRequest(
    Guid DeleganteId,
    Guid DelegadoId,
    DateTime InicioUtc,
    DateTime FinUtc,
    bool AplicaTodosLosRoles,
    IReadOnlyCollection<Guid> Roles,
    string? MetodoMfa);

public sealed record UpdateDelegacionTemporalRequest(
    Guid DelegacionId,
    DateTime InicioUtc,
    DateTime FinUtc,
    bool AplicaTodosLosRoles,
    IReadOnlyCollection<Guid> Roles);
