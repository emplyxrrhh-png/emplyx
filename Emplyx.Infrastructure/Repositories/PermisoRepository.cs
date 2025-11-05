using Emplyx.Domain.Entities.Permisos;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class PermisoRepository : RepositoryBase<Permiso>, IPermisoRepository
{
    public PermisoRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<Permiso?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Codigo == codigo, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permiso>> GetByModuloAsync(Guid moduloId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.ModuloId == moduloId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
