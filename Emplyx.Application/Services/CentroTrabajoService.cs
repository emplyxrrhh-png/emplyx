using System;
using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.CentrosTrabajo;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;

namespace Emplyx.Application.Services;

internal sealed class CentroTrabajoService : ICentroTrabajoService
{
    private readonly ICentroTrabajoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CentroTrabajoService(ICentroTrabajoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CentroTrabajo>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByEmpresaIdAsync(empresaId, cancellationToken);
    }

    public async Task<List<CentroTrabajo>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByTenantIdAsync(tenantId, cancellationToken);
    }

    public async Task<List<CentroTrabajo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<CentroTrabajo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<CentroTrabajo> CreateAsync(CentroTrabajo centroTrabajo, CancellationToken cancellationToken = default)
    {
        _repository.Add(centroTrabajo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return centroTrabajo;
    }

    public async Task UpdateAsync(CentroTrabajo centroTrabajo, CancellationToken cancellationToken = default)
    {
        _repository.Update(centroTrabajo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var centro = await _repository.GetByIdAsync(id, cancellationToken);
        if (centro != null)
        {
            _repository.Remove(centro);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
