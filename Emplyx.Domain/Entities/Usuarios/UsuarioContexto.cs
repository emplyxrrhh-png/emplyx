namespace Emplyx.Domain.Entities.Usuarios;

public sealed class UsuarioContexto
{
    private UsuarioContexto()
    {
    }

    internal UsuarioContexto(Guid usuarioId, Guid contextoId, DateTime linkedAtUtc, bool isPrimary)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("User id must be provided.", nameof(usuarioId));
        }

        if (contextoId == Guid.Empty)
        {
            throw new ArgumentException("Context id must be provided.", nameof(contextoId));
        }

        UsuarioId = usuarioId;
        ContextoId = contextoId;
        LinkedAtUtc = linkedAtUtc;
        IsPrimary = isPrimary;
    }

    public Guid UsuarioId { get; private set; }

    public Guid ContextoId { get; private set; }

    public DateTime LinkedAtUtc { get; private set; }

    public bool IsPrimary { get; private set; }

    internal void MarkAsSecondary() => IsPrimary = false;

    internal void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
}
