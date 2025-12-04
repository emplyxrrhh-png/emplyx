using Emplyx.Domain.Entities.Employees;

namespace Emplyx.Domain.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<List<Employee>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default);
    Task<List<Employee>> GetByCentroTrabajoIdAsync(Guid centroTrabajoId, CancellationToken cancellationToken = default);
}
