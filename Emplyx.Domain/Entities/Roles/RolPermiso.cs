using Emplyx.Domain.Entities.Permisos;

namespace Emplyx.Domain.Entities.Roles;

public sealed class RolPermiso
{
    private RolPermiso()
    {
    }

    internal RolPermiso(Guid rolId, Guid permisoId, DateTime grantedAtUtc)
    {
        if (rolId == Guid.Empty)
        {
            throw new ArgumentException("Role id must be provided.", nameof(rolId));
        }

        if (permisoId == Guid.Empty)
        {
            throw new ArgumentException("Permission id must be provided.", nameof(permisoId));
        }

        RolId = rolId;
        PermisoId = permisoId;
        GrantedAtUtc = grantedAtUtc;
    }

    public Guid RolId { get; private set; }

    public Guid PermisoId { get; private set; }

    public Permiso Permiso { get; private set; } = null!;

    public DateTime GrantedAtUtc { get; private set; }
}
