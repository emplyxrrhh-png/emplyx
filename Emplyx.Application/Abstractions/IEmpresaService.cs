using Emplyx.Shared.Contracts.Empresas;

namespace Emplyx.Application.Abstractions;

public interface IEmpresaService
{
    Task<List<EmpresaResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<EmpresaResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<EmpresaResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmpresaResponse> CreateAsync(CreateEmpresaRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateEmpresaRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
