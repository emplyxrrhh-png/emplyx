namespace Emplyx.Domain.Entities.Usuarios;

public sealed class UsuarioLicencia
{
    private UsuarioLicencia()
    {
    }

    internal UsuarioLicencia(Guid usuarioId, Guid licenciaId, DateTime assignedAtUtc)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("User id must be provided.", nameof(usuarioId));
        }

        if (licenciaId == Guid.Empty)
        {
            throw new ArgumentException("License id must be provided.", nameof(licenciaId));
        }

        UsuarioId = usuarioId;
        LicenciaId = licenciaId;
        AssignedAtUtc = assignedAtUtc;
    }

    public Guid UsuarioId { get; private set; }

    public Guid LicenciaId { get; private set; }

    public DateTime AssignedAtUtc { get; private set; }
}
