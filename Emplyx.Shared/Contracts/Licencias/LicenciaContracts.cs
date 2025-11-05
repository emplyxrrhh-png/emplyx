namespace Emplyx.Shared.Contracts.Licencias;

public sealed record LicenciaDto(
    Guid Id,
    string Codigo,
    string Nombre,
    DateTime InicioVigenciaUtc,
    DateTime? FinVigenciaUtc,
    int? LimiteUsuarios,
    bool EsTrial,
    bool IsRevoked,
    IReadOnlyCollection<Guid> Modulos);

public sealed record UpsertLicenciaRequest(
    Guid? Id,
    string Codigo,
    string Nombre,
    DateTime InicioVigenciaUtc,
    DateTime? FinVigenciaUtc,
    int? LimiteUsuarios,
    bool EsTrial,
    IReadOnlyCollection<Guid> Modulos);
