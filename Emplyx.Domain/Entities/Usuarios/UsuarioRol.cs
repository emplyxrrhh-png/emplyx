using Emplyx.Domain.Entities.Roles;

namespace Emplyx.Domain.Entities.Usuarios;

public sealed class UsuarioRol
{
    private UsuarioRol()
    {
    }

    internal UsuarioRol(Guid usuarioId, Guid rolId, DateTime assignedAtUtc)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("User id must be provided.", nameof(usuarioId));
        }

        if (rolId == Guid.Empty)
        {
            throw new ArgumentException("Role id must be provided.", nameof(rolId));
        }

        UsuarioId = usuarioId;
        RolId = rolId;
        AssignedAtUtc = assignedAtUtc;
    }

    public Guid UsuarioId { get; private set; }

    public Guid RolId { get; private set; }

    public Rol Rol { get; private set; } = null!;

    public DateTime AssignedAtUtc { get; private set; }
}
