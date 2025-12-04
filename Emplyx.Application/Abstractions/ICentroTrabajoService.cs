using Emplyx.Domain.Entities.CentrosTrabajo;

namespace Emplyx.Application.Abstractions;

public interface ICentroTrabajoService
{
    Task<List<CentroTrabajo>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default);
    Task<List<CentroTrabajo>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<CentroTrabajo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CentroTrabajo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CentroTrabajo> CreateAsync(CentroTrabajo centroTrabajo, CancellationToken cancellationToken = default);
    Task UpdateAsync(CentroTrabajo centroTrabajo, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
