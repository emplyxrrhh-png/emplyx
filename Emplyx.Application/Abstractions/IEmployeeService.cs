using Emplyx.Domain.Entities.Employees;

namespace Emplyx.Application.Abstractions;

public interface IEmployeeService
{
    Task<List<Employee>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default);
    Task<List<Employee>> GetByCentroTrabajoIdAsync(Guid centroTrabajoId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
