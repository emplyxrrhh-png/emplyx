using Emplyx.Domain.Entities.Empresas;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class EmpresaRepository : RepositoryBase<Empresa>, IEmpresaRepository
{
    public EmpresaRepository(EmplyxDbContext context)
        : base(context)
    {
    }

    public async Task<List<Empresa>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }
}
