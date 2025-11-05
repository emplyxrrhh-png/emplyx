namespace Emplyx.Domain.Entities.Licencias;

public sealed class LicenciaModulo
{
    private LicenciaModulo()
    {
    }

    internal LicenciaModulo(Guid licenciaId, Guid moduloId, DateTime linkedAtUtc)
    {
        if (licenciaId == Guid.Empty)
        {
            throw new ArgumentException("License id must be provided.", nameof(licenciaId));
        }

        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id must be provided.", nameof(moduloId));
        }

        LicenciaId = licenciaId;
        ModuloId = moduloId;
        LinkedAtUtc = linkedAtUtc;
    }

    public Guid LicenciaId { get; private set; }

    public Guid ModuloId { get; private set; }

    public DateTime LinkedAtUtc { get; private set; }
}
