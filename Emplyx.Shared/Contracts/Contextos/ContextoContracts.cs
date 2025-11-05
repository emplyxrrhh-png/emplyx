namespace Emplyx.Shared.Contracts.Contextos;

public sealed record ContextoDto(
    Guid Id,
    string Clave,
    string Nombre,
    string? Descripcion,
    Guid DelegacionId,
    Guid? LicenciaId,
    Guid? ClearanceId,
    bool IsActive,
    IReadOnlyCollection<ContextoModuloDto> Modulos);

public sealed record ContextoModuloDto(Guid ModuloId, DateTime HabilitadoDesdeUtc, DateTime? HabilitadoHastaUtc);

public sealed record UpsertContextoRequest(
    Guid? Id,
    string Clave,
    string Nombre,
    string? Descripcion,
    Guid DelegacionId,
    Guid? LicenciaId,
    Guid? ClearanceId,
    bool IsActive,
    IReadOnlyCollection<ContextoModuloAssignmentDto> Modulos);

public sealed record ContextoModuloAssignmentDto(Guid ModuloId, DateTime? HabilitadoHastaUtc);
