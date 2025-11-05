using Emplyx.Domain.Entities.Licencias;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class LicenciaRepository : RepositoryBase<Licencia>, ILicenciaRepository
{
    public LicenciaRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<Licencia?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(l => l.Modulos)
            .AsNoTracking()
            .SingleOrDefaultAsync(l => l.Codigo == codigo, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Licencia>> GetExpiringWithinAsync(TimeSpan window, CancellationToken cancellationToken = default)
    {
        if (window <= TimeSpan.Zero)
        {
            return Array.Empty<Licencia>();
        }

        var cutoffDate = DateTime.UtcNow.Add(window);

        return await DbSet
            .Where(l => !l.IsRevoked && l.FinVigenciaUtc != null && l.FinVigenciaUtc <= cutoffDate)
            .Include(l => l.Modulos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
