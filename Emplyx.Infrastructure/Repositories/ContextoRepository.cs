using Emplyx.Domain.Entities.Contextos;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class ContextoRepository : RepositoryBase<Contexto>, IContextoRepository
{
    public ContextoRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public override async Task<Contexto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Modulos)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Contexto?> GetByClaveAsync(string clave, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Modulos)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Clave == clave, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Contexto>> GetByDelegacionAsync(Guid delegacionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.DelegacionId == delegacionId)
            .Include(c => c.Modulos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
