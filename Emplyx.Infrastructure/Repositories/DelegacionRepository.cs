using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class DelegacionRepository : RepositoryBase<Delegacion>, IDelegacionRepository
{
    public DelegacionRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public override async Task<Delegacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Roles)
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Delegacion?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Roles)
            .AsNoTracking()
            .SingleOrDefaultAsync(d => d.Codigo == codigo, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Delegacion>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Roles)
            .Where(d => d.ParentId == parentId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Delegacion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Roles)
            .AsNoTracking()
            .OrderBy(d => d.Nombre)
            .ToListAsync(cancellationToken);
    }
}
