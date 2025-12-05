using Emplyx.Domain.Entities.Permisos;
using Emplyx.Domain.Entities.Roles;
using Emplyx.Domain.Entities.Usuarios;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public override async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.Roles)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .Include(u => u.Sesiones)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.Roles)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.Roles)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Usuario>> SearchAsync(
        string? userNameOrEmail,
        Guid? contextoId,
        Guid? rolId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Usuario> query = DbSet;

        if (!string.IsNullOrWhiteSpace(userNameOrEmail))
        {
            var term = userNameOrEmail.Trim();
            query = query.Where(u => EF.Functions.Like(u.UserName, $"%{term}%") ||
                                     EF.Functions.Like(u.Email, $"%{term}%"));
        }

        if (contextoId.HasValue)
        {
            query = query.Where(u => u.Contextos.Any(c => c.ContextoId == contextoId));
        }

        if (rolId.HasValue)
        {
            query = query.Where(u => u.Roles.Any(r => r.RolId == rolId));
        }

        return await query
            .Include(u => u.Roles)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPermissionAsync(Guid usuarioId, string permission, Guid? contextoId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<UsuarioRol>()
            .Where(ur => ur.UsuarioId == usuarioId && 
                         (ur.ContextoId == Guid.Empty || (contextoId.HasValue && ur.ContextoId == contextoId.Value)))
            .Join(Context.Set<RolPermiso>(),
                ur => ur.RolId,
                rp => rp.RolId,
                (ur, rp) => rp)
            .Join(Context.Set<Permiso>(),
                rp => rp.PermisoId,
                p => p.Id,
                (rp, p) => p)
            .AnyAsync(p => p.Codigo == permission, cancellationToken);
    }
}
