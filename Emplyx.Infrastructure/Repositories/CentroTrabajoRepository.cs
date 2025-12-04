using System;
using Emplyx.Domain.Entities.CentrosTrabajo;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class CentroTrabajoRepository : RepositoryBase<CentroTrabajo>, ICentroTrabajoRepository
{
    public CentroTrabajoRepository(EmplyxDbContext context) : base(context)
    {
    }

    public async Task<List<CentroTrabajo>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.EmpresaId == empresaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CentroTrabajo>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await Context.CentrosTrabajo
            .Join(
                Context.Empresas,
                centro => centro.EmpresaId,
                empresa => empresa.Id,
                (centro, empresa) => new { centro, empresa })
            .Where(joined => joined.empresa.TenantId == tenantId)
            .Select(joined => joined.centro)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CentroTrabajo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public void Add(CentroTrabajo centroTrabajo)
    {
        DbSet.Add(centroTrabajo);
    }

    public void Update(CentroTrabajo centroTrabajo)
    {
        DbSet.Update(centroTrabajo);
    }

    public void Remove(CentroTrabajo centroTrabajo)
    {
        DbSet.Remove(centroTrabajo);
    }
}
