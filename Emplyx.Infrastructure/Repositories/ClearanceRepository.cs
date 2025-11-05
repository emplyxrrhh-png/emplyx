using Emplyx.Domain.Entities.Clearances;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class ClearanceRepository : RepositoryBase<Clearance>, IClearanceRepository
{
    public ClearanceRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<Clearance?> GetByNivelAsync(int nivel, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Nivel == nivel, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Clearance>> GetActivasAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsActive)
            .AsNoTracking()
            .OrderBy(c => c.Nivel)
            .ToListAsync(cancellationToken);
    }
}
