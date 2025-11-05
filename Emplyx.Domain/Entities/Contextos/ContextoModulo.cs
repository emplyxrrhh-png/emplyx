namespace Emplyx.Domain.Entities.Contextos;

public sealed class ContextoModulo
{
    private ContextoModulo()
    {
    }

    internal ContextoModulo(Guid contextoId, Guid moduloId, DateTime habilitadoDesdeUtc, DateTime? habilitadoHastaUtc)
    {
        if (contextoId == Guid.Empty)
        {
            throw new ArgumentException("Context id must be provided.", nameof(contextoId));
        }

        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id must be provided.", nameof(moduloId));
        }

        ContextoId = contextoId;
        ModuloId = moduloId;
        HabilitadoDesdeUtc = habilitadoDesdeUtc;
        HabilitadoHastaUtc = habilitadoHastaUtc;
    }

    public Guid ContextoId { get; private set; }

    public Guid ModuloId { get; private set; }

    public DateTime HabilitadoDesdeUtc { get; private set; }

    public DateTime? HabilitadoHastaUtc { get; private set; }

    public void Finalizar(DateTime fecha)
    {
        HabilitadoHastaUtc = fecha;
    }
}
