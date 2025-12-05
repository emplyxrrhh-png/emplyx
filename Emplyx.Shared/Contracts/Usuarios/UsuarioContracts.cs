namespace Emplyx.Shared.Contracts.Usuarios;

public sealed record UsuarioDto(
    Guid Id,
    string UserName,
    string Email,
    string DisplayName,
    bool IsActive,
    Guid? ClearanceId,
    string? ExternalIdentityId,
    Guid? PreferredContextoId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? LastLoginAtUtc,
    DateTime? LastPasswordChangeAtUtc,
    UsuarioPerfilDto Perfil,
    IReadOnlyCollection<UsuarioRolDto> Roles,
    IReadOnlyCollection<UsuarioContextoDto> Contextos,
    IReadOnlyCollection<UsuarioLicenciaDto> Licencias,
    IReadOnlyCollection<UsuarioSesionDto> Sesiones);

public sealed record UsuarioPerfilDto(
    string? Nombres,
    string? Apellidos,
    string? Departamento,
    string? Cargo,
    string? Telefono);

public sealed record UsuarioRolDto(Guid RolId, Guid? ContextoId, DateTime AssignedAtUtc);

public sealed record UsuarioContextoDto(Guid ContextoId, bool IsPrimary, DateTime LinkedAtUtc);

public sealed record UsuarioLicenciaDto(Guid LicenciaId, DateTime AssignedAtUtc);

public sealed record UsuarioSesionDto(Guid Id, string Device, string? IpAddress, DateTime CreatedAtUtc, DateTime ExpiresAtUtc, bool IsActive, DateTime? ClosedAtUtc);

public sealed record UsuarioRolAssignmentDto(Guid RolId, Guid? ContextoId);

public sealed record CreateUsuarioRequest(
    string UserName,
    string Email,
    string DisplayName,
    string? PasswordHash,
    string? ExternalIdentityId,
    Guid? ClearanceId,
    Guid? PreferredContextoId,
    UsuarioPerfilDto Perfil,
    IReadOnlyCollection<UsuarioRolAssignmentDto> Roles,
    IReadOnlyCollection<UsuarioContextoAssignmentDto> Contextos,
    IReadOnlyCollection<Guid> Licencias);

public sealed record UpdateUsuarioRequest(
    Guid UsuarioId,
    string DisplayName,
    string Email,
    bool IsActive,
    Guid? ClearanceId,
    string? PasswordHash,
    string? ExternalIdentityId,
    Guid? PreferredContextoId,
    UsuarioPerfilDto Perfil,
    IReadOnlyCollection<UsuarioRolAssignmentDto> Roles,
    IReadOnlyCollection<UsuarioContextoAssignmentDto> Contextos,
    IReadOnlyCollection<Guid> Licencias);

public sealed record SearchUsuariosRequest(
    string? UserNameOrEmail,
    Guid? ContextoId,
    Guid? RolId);

public sealed record LoginRequest(string UserNameOrEmail, string Password);

public sealed record UsuarioContextoAssignmentDto(Guid ContextoId, bool IsPrimary);
