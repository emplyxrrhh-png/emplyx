using Emplyx.Domain.Entities.Employees;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal sealed class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(EmplyxDbContext context) : base(context)
    {
    }

    public async Task<List<Employee>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.UserFields)
            .Where(e => e.EmpresaId == empresaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> GetByCentroTrabajoIdAsync(Guid centroTrabajoId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.UserFields)
            .Where(e => e.CentroTrabajoId == centroTrabajoId)
            .ToListAsync(cancellationToken);
    }
}
