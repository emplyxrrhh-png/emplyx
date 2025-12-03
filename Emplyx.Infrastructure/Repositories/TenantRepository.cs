using Emplyx.Domain.Entities.Tenants;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class TenantRepository : RepositoryBase<Tenant>, ITenantRepository
{
    public TenantRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }
}
