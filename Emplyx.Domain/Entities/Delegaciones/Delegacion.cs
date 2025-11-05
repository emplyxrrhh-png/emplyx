using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Delegaciones;

public sealed class Delegacion : Entity, IAggregateRoot
{
    private readonly HashSet<DelegacionRol> _roles = new();

    private Delegacion()
    {
    }

    public Delegacion(Guid id, string nombre, string? codigo = null, string? descripcion = null, Guid? parentId = null)
        : base(id)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Delegation name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Codigo = codigo?.Trim();
        Descripcion = descripcion?.Trim();
        ParentId = parentId;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Nombre { get; private set; } = string.Empty;

    public string? Codigo { get; private set; }

    public string? Descripcion { get; private set; }

    public Guid? ParentId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<DelegacionRol> Roles => _roles;

    public void UpdateMetadata(string nombre, string? codigo, string? descripcion, Guid? parentId)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Delegation name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Codigo = codigo?.Trim();
        Descripcion = descripcion?.Trim();
        ParentId = parentId;
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

    public void AttachRol(Guid rolId)
    {
        if (_roles.Any(r => r.RolId == rolId))
        {
            return;
        }

        _roles.Add(new DelegacionRol(Id, rolId));
        Touch();
    }

    public void DetachRol(Guid rolId)
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
