using Emplyx.Domain.Entities.Tenants;

namespace Emplyx.Domain.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
}
