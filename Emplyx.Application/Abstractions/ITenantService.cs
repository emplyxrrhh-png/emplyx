using Emplyx.Shared.Contracts.Tenants;

namespace Emplyx.Application.Abstractions;

public interface ITenantService
{
    Task<List<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
