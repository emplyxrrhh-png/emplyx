using Emplyx.Application.Abstractions;
using Emplyx.Application.Mappers;
using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Shared.Contracts.Delegaciones;

namespace Emplyx.Application.Services;

internal sealed class DelegacionService : IDelegacionService
{
    private readonly IDelegacionRepository _delegacionRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DelegacionService(
        IDelegacionRepository delegacionRepository,
        IRolRepository rolRepository,
        IUnitOfWork unitOfWork)
    {
        _delegacionRepository = delegacionRepository;
        _rolRepository = rolRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DelegacionDto> UpsertAsync(UpsertDelegacionRequest request, CancellationToken cancellationToken = default)
    {
        Delegacion delegacion;

        if (request.Id.HasValue)
        {
            delegacion = await _delegacionRepository.GetByIdAsync(request.Id.Value, cancellationToken)
                         ?? throw new InvalidOperationException("La delegacion especificada no existe.");

            delegacion.UpdateMetadata(request.Nombre, request.Codigo, request.Descripcion, request.ParentId);

            if (request.IsActive)
            {
                delegacion.Activate();
            }
            else
            {
                delegacion.Deactivate();
            }
        }
        else
        {
            delegacion = new Delegacion(Guid.NewGuid(), request.Nombre, request.Codigo, request.Descripcion, request.ParentId);
            await _delegacionRepository.AddAsync(delegacion, cancellationToken);
        }

        await SyncRolesAsync(delegacion, request.Roles, cancellationToken);

        _delegacionRepository.Update(delegacion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return delegacion.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var delegacion = await _delegacionRepository.GetByIdAsync(id, cancellationToken);
        if (delegacion is null)
        {
            return false;
        }

        _delegacionRepository.Remove(delegacion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<DelegacionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var delegacion = await _delegacionRepository.GetByIdAsync(id, cancellationToken);
        return delegacion?.ToDto();
    }

    public async Task<IReadOnlyCollection<DelegacionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var delegaciones = await _delegacionRepository.GetAllAsync(cancellationToken);
        return delegaciones.Select(d => d.ToDto()).ToArray();
    }

    private async Task SyncRolesAsync(Delegacion delegacion, IEnumerable<Guid> roles, CancellationToken cancellationToken)
    {
        var desired = (roles ?? Array.Empty<Guid>()).Distinct().ToHashSet();
        var current = delegacion.Roles.Select(r => r.RolId).ToHashSet();

        foreach (var toRemove in current.Except(desired).ToArray())
        {
            delegacion.DetachRol(toRemove);
        }

        foreach (var toAdd in desired.Except(current))
        {
            var rol = await _rolRepository.GetByIdAsync(toAdd, cancellationToken);
            if (rol is null)
            {
                throw new InvalidOperationException($"El rol {toAdd} no existe.");
            }

            delegacion.AttachRol(toAdd);
        }
    }
}
