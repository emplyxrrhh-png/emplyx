using Emplyx.Domain.Entities.DelegacionesTemporales;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class DelegacionTemporalRepository : RepositoryBase<DelegacionTemporal>, IDelegacionTemporalRepository
{
    public DelegacionTemporalRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public override async Task<DelegacionTemporal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dt => dt.Roles)
            .SingleOrDefaultAsync(dt => dt.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DelegacionTemporal>> GetPendientesAsync(Guid delegadoId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dt => dt.Roles)
            .Where(dt => dt.DelegadoId == delegadoId && dt.Estado == DelegacionTemporalEstado.Pendiente)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DelegacionTemporal>> GetActivasAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dt => dt.Roles)
            .Where(dt => dt.DelegadoId == usuarioId && dt.Estado == DelegacionTemporalEstado.Activa)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
