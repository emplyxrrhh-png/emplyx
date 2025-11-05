namespace Emplyx.Domain.Entities.Delegaciones;

public sealed class DelegacionRol
{
    private DelegacionRol()
    {
    }

    internal DelegacionRol(Guid delegacionId, Guid rolId)
    {
        if (delegacionId == Guid.Empty)
        {
            throw new ArgumentException("Delegation id must be provided.", nameof(delegacionId));
        }

        if (rolId == Guid.Empty)
        {
            throw new ArgumentException("Role id must be provided.", nameof(rolId));
        }

        DelegacionId = delegacionId;
        RolId = rolId;
        LinkedAtUtc = DateTime.UtcNow;
    }

    public Guid DelegacionId { get; private set; }

    public Guid RolId { get; private set; }

    public DateTime LinkedAtUtc { get; private set; }
}
