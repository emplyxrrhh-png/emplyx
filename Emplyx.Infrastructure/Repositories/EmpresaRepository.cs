using System;
using Emplyx.Domain.Entities.Empresas;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class EmpresaRepository : RepositoryBase<Empresa>, IEmpresaRepository
{
    public EmpresaRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<List<Empresa>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task<List<Empresa>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }
}
