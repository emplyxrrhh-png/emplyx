using Emplyx.Domain.Entities.CentrosTrabajo;

namespace Emplyx.Domain.Repositories;

public interface ICentroTrabajoRepository
{
    Task<CentroTrabajo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<CentroTrabajo>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default);
    Task<List<CentroTrabajo>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<CentroTrabajo>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(CentroTrabajo centroTrabajo);
    void Update(CentroTrabajo centroTrabajo);
    void Remove(CentroTrabajo centroTrabajo);
}
