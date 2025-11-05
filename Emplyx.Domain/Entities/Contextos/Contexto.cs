using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Contextos;

public sealed class Contexto : Entity, IAggregateRoot
{
    private readonly HashSet<ContextoModulo> _modulos = new();

    private Contexto()
    {
    }

    public Contexto(
        Guid id,
        string clave,
        string nombre,
        Guid delegacionId,
        Guid? licenciaId = null,
        Guid? clearanceId = null,
        string? descripcion = null)
        : base(id)
    {
        Clave = string.IsNullOrWhiteSpace(clave)
            ? throw new ArgumentException("Context key cannot be empty.", nameof(clave))
            : clave.Trim().ToUpperInvariant();

        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Context name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Descripcion = descripcion?.Trim();
        DelegacionId = delegacionId == Guid.Empty
            ? throw new ArgumentException("Delegation id must be provided.", nameof(delegacionId))
            : delegacionId;

        LicenciaId = licenciaId;
        ClearanceId = clearanceId;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Clave { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public Guid DelegacionId { get; private set; }

    public Guid? LicenciaId { get; private set; }

    public Guid? ClearanceId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<ContextoModulo> Modulos => _modulos;

    public void UpdateMetadata(string nombre, string? descripcion)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Context name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Descripcion = descripcion?.Trim();
        Touch();
    }

    public void ChangeDelegacion(Guid delegacionId)
    {
        if (delegacionId == Guid.Empty)
        {
            throw new ArgumentException("Delegation id must be provided.", nameof(delegacionId));
        }

        DelegacionId = delegacionId;
        Touch();
    }

    public void AssignLicencia(Guid? licenciaId)
    {
        if (licenciaId == Guid.Empty)
        {
            throw new ArgumentException("License id must be provided.", nameof(licenciaId));
        }

        LicenciaId = licenciaId;
        Touch();
    }

    public void SetClearance(Guid? clearanceId)
    {
        if (clearanceId == Guid.Empty)
        {
            throw new ArgumentException("Clearance id must be provided.", nameof(clearanceId));
        }

        ClearanceId = clearanceId;
        Touch();
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        Touch();
    }

    public void AttachModulo(Guid moduloId, DateTime? habilitadoHastaUtc = null)
    {
        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id must be provided.", nameof(moduloId));
        }

        if (_modulos.Any(m => m.ModuloId == moduloId))
        {
            return;
        }

        _modulos.Add(new ContextoModulo(Id, moduloId, DateTime.UtcNow, habilitadoHastaUtc));
        Touch();
    }

    public void DetachModulo(Guid moduloId)
    {
        var modulo = _modulos.SingleOrDefault(m => m.ModuloId == moduloId);
        if (modulo is null)
        {
            return;
        }

        _modulos.Remove(modulo);
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
