using Emplyx.Domain.Entities.Modulos;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class ModuloRepository : RepositoryBase<Modulo>, IModuloRepository
{
    public ModuloRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<Modulo?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(m => m.Codigo == codigo, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Modulo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }
}
