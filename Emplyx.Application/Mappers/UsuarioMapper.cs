using Emplyx.Domain.Entities.Usuarios;
using Emplyx.Shared.Contracts.Usuarios;

namespace Emplyx.Application.Mappers;

internal static class UsuarioMapper
{
    public static UsuarioDto ToDto(this Usuario entity)
    {
        var perfil = new UsuarioPerfilDto(
            entity.Perfil.Nombres,
            entity.Perfil.Apellidos,
            entity.Perfil.Departamento,
            entity.Perfil.Cargo,
            entity.Perfil.Telefono);

        return new UsuarioDto(
            entity.Id,
            entity.UserName,
            entity.Email,
            entity.DisplayName,
            entity.IsActive,
            entity.ClearanceId,
            entity.ExternalIdentityId,
            entity.PreferredContextoId,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc,
            entity.LastLoginAtUtc,
            entity.LastPasswordChangeAtUtc,
            perfil,
            entity.Roles.Select(r => new UsuarioRolDto(r.RolId, r.Rol?.Nombre, r.AssignedAtUtc)).ToArray(),
            entity.Contextos.Select(c => new UsuarioContextoDto(c.ContextoId, c.IsPrimary, c.LinkedAtUtc)).ToArray(),
            entity.Licencias.Select(l => new UsuarioLicenciaDto(l.LicenciaId, l.AssignedAtUtc)).ToArray(),
            entity.Sesiones.Select(s => new UsuarioSesionDto(s.Id, s.Device, s.IpAddress, s.CreatedAtUtc, s.ExpiresAtUtc, s.IsActive, s.ClosedAtUtc)).ToArray());
    }
}
