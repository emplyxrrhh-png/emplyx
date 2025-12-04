using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.Employees;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;

namespace Emplyx.Application.Services;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IEmployeeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Employee>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByEmpresaIdAsync(empresaId, cancellationToken);
    }

    public async Task<List<Employee>> GetByCentroTrabajoIdAsync(Guid centroTrabajoId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByCentroTrabajoIdAsync(centroTrabajoId, cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _repository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee != null)
        {
            _repository.Remove(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
