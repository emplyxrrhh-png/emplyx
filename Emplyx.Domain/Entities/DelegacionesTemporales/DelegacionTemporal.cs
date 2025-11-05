using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.DelegacionesTemporales;

public sealed class DelegacionTemporal : Entity, IAggregateRoot
{
    private readonly HashSet<DelegacionTemporalRol> _roles = new();

    private DelegacionTemporal()
        : base(Guid.Empty)
    {
    }

    public DelegacionTemporal(
        Guid id,
        Guid deleganteId,
        Guid delegadoId,
        DateTime inicioUtc,
        DateTime finUtc,
        bool aplicaTodosLosRoles)
        : base(id)
    {
        if (deleganteId == Guid.Empty)
        {
            throw new ArgumentException("Delegating user id cannot be empty.", nameof(deleganteId));
        }

        if (delegadoId == Guid.Empty)
        {
            throw new ArgumentException("Delegate user id cannot be empty.", nameof(delegadoId));
        }

        if (deleganteId == delegadoId)
        {
            throw new ArgumentException("Delegating and delegate users must differ.");
        }

        if (finUtc <= inicioUtc)
        {
            throw new ArgumentException("End date must be greater than start date.", nameof(finUtc));
        }

        DeleganteId = deleganteId;
        DelegadoId = delegadoId;
        InicioUtc = inicioUtc;
        FinUtc = finUtc;
        AplicaTodosLosRoles = aplicaTodosLosRoles;
        Estado = DelegacionTemporalEstado.Pendiente;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid DeleganteId { get; private set; }

    public Guid DelegadoId { get; private set; }

    public DateTime InicioUtc { get; private set; }

    public DateTime FinUtc { get; private set; }

    public bool AplicaTodosLosRoles { get; private set; }

    public bool AprobadaMfa { get; private set; }

    public string? MetodoMfa { get; private set; }

    public DelegacionTemporalEstado Estado { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public DateTime? RevocadaUtc { get; private set; }

    public DateTime? ExpiradaUtc { get; private set; }

    public IReadOnlyCollection<DelegacionTemporalRol> Roles => _roles;

    public void AprobarMfa(string metodo)
    {
        if (string.IsNullOrWhiteSpace(metodo))
        {
            throw new ArgumentException("MFA method must not be empty.", nameof(metodo));
        }

        AprobadaMfa = true;
        MetodoMfa = metodo.Trim();
        Touch();
    }

    public void Activar()
    {
        if (Estado == DelegacionTemporalEstado.Revocada || Estado == DelegacionTemporalEstado.Expirada)
        {
            throw new InvalidOperationException("Cannot activate a delegation that is no longer valid.");
        }

        Estado = DelegacionTemporalEstado.Activa;
        Touch();
    }

    public void Revocar()
    {
        if (Estado == DelegacionTemporalEstado.Revocada)
        {
            return;
        }

        Estado = DelegacionTemporalEstado.Revocada;
        RevocadaUtc = DateTime.UtcNow;
        Touch();
    }

    public void Expirar()
    {
        if (Estado == DelegacionTemporalEstado.Expirada)
        {
            return;
        }

        Estado = DelegacionTemporalEstado.Expirada;
        ExpiradaUtc = DateTime.UtcNow;
        Touch();
    }

    public void ActualizarVentana(DateTime inicioUtc, DateTime finUtc)
    {
        if (finUtc <= inicioUtc)
        {
            throw new ArgumentException("End date must be greater than start date.", nameof(finUtc));
        }

        InicioUtc = inicioUtc;
        FinUtc = finUtc;
        Touch();
    }

    public void EstablecerCoberturaDeRoles(bool aplicaTodosLosRoles)
    {
        AplicaTodosLosRoles = aplicaTodosLosRoles;
        Touch();
    }

    public void AdjuntarRol(Guid rolId)
    {
        if (rolId == Guid.Empty)
        {
            throw new ArgumentException("Role id must be provided.", nameof(rolId));
        }

        if (_roles.Any(r => r.RolId == rolId))
        {
            return;
        }

        _roles.Add(new DelegacionTemporalRol(Id, rolId));
        Touch();
    }

    public void RemoverRol(Guid rolId)
    {
        var rol = _roles.SingleOrDefault(r => r.RolId == rolId);
        if (rol is null)
        {
            return;
        }

        _roles.Remove(rol);
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
