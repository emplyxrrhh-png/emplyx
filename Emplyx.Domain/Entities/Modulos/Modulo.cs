using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Modulos;

public sealed class Modulo : Entity, IAggregateRoot
{
    private Modulo()
    {
    }

    public Modulo(Guid id, string codigo, string nombre, string? descripcion = null, bool esCritico = false)
        : base(id)
    {
        Codigo = string.IsNullOrWhiteSpace(codigo)
            ? throw new ArgumentException("Module code cannot be empty.", nameof(codigo))
            : codigo.Trim().ToUpperInvariant();

        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Module name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Descripcion = descripcion?.Trim();
        EsCritico = esCritico;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public bool EsCritico { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdateMetadata(string nombre, string? descripcion, bool esCritico)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Module name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Descripcion = descripcion?.Trim();
        EsCritico = esCritico;
        Touch();
    }

    public void UpdateCodigo(string codigo)
    {
        Codigo = string.IsNullOrWhiteSpace(codigo)
            ? throw new ArgumentException("Module code cannot be empty.", nameof(codigo))
            : codigo.Trim().ToUpperInvariant();
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
