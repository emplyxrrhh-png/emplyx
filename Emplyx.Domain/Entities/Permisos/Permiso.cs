using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Permisos;

public sealed class Permiso : Entity, IAggregateRoot
{
    private Permiso()
    {
    }

    public Permiso(Guid id, Guid moduloId, string codigo, string nombre, string? categoria = null, bool esCritico = false)
        : base(id)
    {
        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id cannot be empty.", nameof(moduloId));
        }

        Codigo = string.IsNullOrWhiteSpace(codigo)
            ? throw new ArgumentException("Permission code cannot be empty.", nameof(codigo))
            : codigo.Trim().ToUpperInvariant();

        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Permission name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Categoria = categoria?.Trim();
        EsCritico = esCritico;
        ModuloId = moduloId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public Guid ModuloId { get; private set; }

    public string? Categoria { get; private set; }

    public bool EsCritico { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdateMetadata(string nombre, Guid moduloId, string? categoria, bool esCritico)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Permission name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id cannot be empty.", nameof(moduloId));
        }

        Categoria = categoria?.Trim();
        EsCritico = esCritico;
        ModuloId = moduloId;
        Touch();
    }

    public void AssignModulo(Guid moduloId)
    {
        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id cannot be empty.", nameof(moduloId));
        }

        ModuloId = moduloId;
        Touch();
    }

    public void UpdateCodigo(string codigo)
    {
        Codigo = string.IsNullOrWhiteSpace(codigo)
            ? throw new ArgumentException("Permission code cannot be empty.", nameof(codigo))
            : codigo.Trim().ToUpperInvariant();
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
