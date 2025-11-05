using Emplyx.Domain.Entities.Roles;
using Emplyx.Shared.Contracts.Roles;

namespace Emplyx.Application.Mappers;

internal static class RolMapper
{
    public static RolDto ToDto(this Rol entity)
    {
        return new RolDto(
            entity.Id,
            entity.Nombre,
            entity.Descripcion,
            entity.IsSystem,
            entity.ClearanceId,
            entity.Permisos.Select(p => p.PermisoId).ToArray(),
            entity.Delegaciones.Select(d => d.DelegacionId).ToArray());
    }
}
