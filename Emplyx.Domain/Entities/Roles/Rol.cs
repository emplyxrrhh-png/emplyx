using Emplyx.Domain.Abstractions;
using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Entities.Permisos;

namespace Emplyx.Domain.Entities.Roles;

public sealed class Rol : Entity, IAggregateRoot
{
    private readonly HashSet<RolPermiso> _permisos = new();
    private readonly HashSet<DelegacionRol> _delegaciones = new();

    private Rol()
    {
    }

    public Rol(Guid id, string nombre, string? descripcion = null, bool isSystem = false, Guid? clearanceId = null)
        : base(id)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Role name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Descripcion = descripcion?.Trim();
        IsSystem = isSystem;
        ClearanceId = clearanceId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public bool IsSystem { get; private set; }

    public Guid? ClearanceId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<RolPermiso> Permisos => _permisos;

    public IReadOnlyCollection<DelegacionRol> Delegaciones => _delegaciones;

    public bool AttachPermiso(Guid permisoId)
    {
        if (permisoId == Guid.Empty)
        {
            throw new ArgumentException("Permission id must be provided.", nameof(permisoId));
        }

        if (_permisos.Any(p => p.PermisoId == permisoId))
        {
            return false;
        }

        _permisos.Add(new RolPermiso(Id, permisoId, DateTime.UtcNow));
        Touch();
        return true;
    }

    public bool DetachPermiso(Guid permisoId)
    {
        var permiso = _permisos.SingleOrDefault(p => p.PermisoId == permisoId);
        if (permiso is null)
        {
            return false;
        }

        _permisos.Remove(permiso);
        Touch();
        return true;
    }

    public void AttachDelegacion(Guid delegacionId)
    {
        if (delegacionId == Guid.Empty)
        {
            throw new ArgumentException("Delegation id must be provided.", nameof(delegacionId));
        }

        if (_delegaciones.Any(d => d.DelegacionId == delegacionId))
        {
            return;
        }

        _delegaciones.Add(new DelegacionRol(delegacionId, Id));
        Touch();
    }

    public void DetachDelegacion(Guid delegacionId)
    {
        var delegacion = _delegaciones.SingleOrDefault(d => d.DelegacionId == delegacionId);
        if (delegacion is null)
        {
            return;
        }

        _delegaciones.Remove(delegacion);
        Touch();
    }

    public void UpdateMetadata(string nombre, string? descripcion, bool isSystem, Guid? clearanceId = null)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("Role name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        Descripcion = descripcion?.Trim();
        IsSystem = isSystem;
        ClearanceId = clearanceId;
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

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
