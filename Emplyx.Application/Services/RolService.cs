using Emplyx.Application.Abstractions;
using Emplyx.Application.Mappers;
using Emplyx.Domain.Entities.Roles;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Shared.Contracts.Roles;

namespace Emplyx.Application.Services;

internal sealed class RolService : IRolService
{
    private readonly IRolRepository _rolRepository;
    private readonly IPermisoRepository _permisoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RolService(
        IRolRepository rolRepository,
        IPermisoRepository permisoRepository,
        IUnitOfWork unitOfWork)
    {
        _rolRepository = rolRepository;
        _permisoRepository = permisoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RolDto> UpsertAsync(UpsertRolRequest request, CancellationToken cancellationToken = default)
    {
        Rol rol;

        if (request.Id.HasValue)
        {
            rol = await _rolRepository.GetByIdAsync(request.Id.Value, cancellationToken)
                  ?? throw new InvalidOperationException("El rol especificado no existe.");

            rol.UpdateMetadata(request.Nombre, request.Descripcion, request.IsSystem, request.ClearanceId);
            await SyncPermisosAsync(rol, request.Permisos, cancellationToken);

            _rolRepository.Update(rol);
        }
        else
        {
            rol = new Rol(Guid.NewGuid(), request.Nombre, request.Descripcion, request.IsSystem, request.ClearanceId);
            await AttachPermisosAsync(rol, request.Permisos, cancellationToken);
            await _rolRepository.AddAsync(rol, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rol.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rol = await _rolRepository.GetByIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return false;
        }

        if (rol.IsSystem)
        {
            throw new InvalidOperationException("No es posible eliminar roles de sistema.");
        }

        _rolRepository.Remove(rol);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<RolDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rol = await _rolRepository.GetByIdAsync(id, cancellationToken);
        return rol?.ToDto();
    }

    public async Task<IReadOnlyCollection<RolDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.GetAllAsync(cancellationToken);
        return roles.Select(r => r.ToDto()).ToArray();
    }

    private async Task AttachPermisosAsync(Rol rol, IEnumerable<Guid> permisos, CancellationToken cancellationToken)
    {
        foreach (var permisoId in (permisos ?? Array.Empty<Guid>()).Distinct())
        {
            var permiso = await _permisoRepository.GetByIdAsync(permisoId, cancellationToken);
            if (permiso is null)
            {
                throw new InvalidOperationException($"El permiso {permisoId} no existe.");
            }

            rol.AttachPermiso(permisoId);
        }
    }

    private async Task SyncPermisosAsync(Rol rol, IEnumerable<Guid> permisos, CancellationToken cancellationToken)
    {
        var desired = (permisos ?? Array.Empty<Guid>()).Distinct().ToHashSet();
        var current = rol.Permisos.Select(p => p.PermisoId).ToHashSet();

        foreach (var toRemove in current.Except(desired).ToArray())
        {
            rol.DetachPermiso(toRemove);
        }

        foreach (var toAdd in desired.Except(current))
        {
            var permiso = await _permisoRepository.GetByIdAsync(toAdd, cancellationToken);
            if (permiso is null)
            {
                throw new InvalidOperationException($"El permiso {toAdd} no existe.");
            }

            rol.AttachPermiso(toAdd);
        }
    }
}
