using Emplyx.Domain.Entities.Usuarios;
using Emplyx.Domain.Entities.Roles;
using Emplyx.Domain.Entities.Permisos;
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
            .Include(u => u.Roles).ThenInclude(ur => ur.Rol)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .Include(u => u.Sesiones)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.Roles).ThenInclude(ur => ur.Rol)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.Roles).ThenInclude(ur => ur.Rol)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, Guid? contextoId = null, CancellationToken cancellationToken = default)
    {
        // If context is specified, ensure user has access to it
        if (contextoId.HasValue)
        {
            var hasContextAccess = await Context.Set<UsuarioContexto>()
                .AnyAsync(uc => uc.UsuarioId == userId && uc.ContextoId == contextoId, cancellationToken);
            
            if (!hasContextAccess)
            {
                return false;
            }
        }

        // Check if user has the permission through any of their roles
        return await Context.Set<Usuario>()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .Select(ur => ur.Rol)
            .SelectMany(r => r.Permisos)
            .Select(rp => rp.Permiso)
            .AnyAsync(p => p.Codigo == permission, cancellationToken);
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
            .Include(u => u.Roles).ThenInclude(ur => ur.Rol)
            .Include(u => u.Contextos)
            .Include(u => u.Licencias)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
