using Emplyx.Domain.Entities.Roles;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class RolRepository : RepositoryBase<Rol>, IRolRepository
{
    public RolRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public override async Task<Rol?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Permisos)
            .Include(r => r.Delegaciones)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Rol?> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Permisos)
            .Include(r => r.Delegaciones)
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Nombre == nombre, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rol>> GetByPermisoAsync(Guid permisoId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Permisos)
            .Include(r => r.Delegaciones)
            .Where(r => r.Permisos.Any(p => p.PermisoId == permisoId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rol>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Permisos)
            .Include(r => r.Delegaciones)
            .AsNoTracking()
            .OrderBy(r => r.Nombre)
            .ToListAsync(cancellationToken);
    }
}
