using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Clearances;

public sealed class Clearance : Entity, IAggregateRoot
{
    private Clearance()
    {
    }

    public Clearance(Guid id, string nombre, int nivel, string? descripcion = null)
        : base(id)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Clearance name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        if (nivel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nivel), "Level must be greater than zero.");
        }

        Nivel = nivel;
        Descripcion = descripcion?.Trim();
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Nombre { get; private set; } = string.Empty;

    public int Nivel { get; private set; }

    public string? Descripcion { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdateMetadata(string nombre, int nivel, string? descripcion)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Clearance name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        if (nivel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nivel), "Level must be greater than zero.");
        }

        Nivel = nivel;
        Descripcion = descripcion?.Trim();
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

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
