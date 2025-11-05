namespace Emplyx.Domain.Entities.DelegacionesTemporales;

public sealed class DelegacionTemporalRol
{
    private DelegacionTemporalRol()
    {
    }

    internal DelegacionTemporalRol(Guid delegacionTemporalId, Guid rolId)
    {
        if (delegacionTemporalId == Guid.Empty)
        {
            throw new ArgumentException("Delegation id must be provided.", nameof(delegacionTemporalId));
        }

        if (rolId == Guid.Empty)
        {
            throw new ArgumentException("Role id must be provided.", nameof(rolId));
        }

        DelegacionTemporalId = delegacionTemporalId;
        RolId = rolId;
    }

    public Guid DelegacionTemporalId { get; private set; }

    public Guid RolId { get; private set; }
}
