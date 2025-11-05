using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Shared.Contracts.Delegaciones;

namespace Emplyx.Application.Mappers;

internal static class DelegacionMapper
{
    public static DelegacionDto ToDto(this Delegacion entity)
    {
        return new DelegacionDto(
            entity.Id,
            entity.Nombre,
            entity.Codigo,
            entity.Descripcion,
            entity.ParentId,
            entity.IsActive,
            entity.Roles.Select(r => r.RolId).ToArray());
    }
}
