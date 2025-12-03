using Emplyx.Domain.Entities.Empresas;

namespace Emplyx.Domain.Repositories;

public interface IEmpresaRepository : IRepository<Empresa>
{
    Task<List<Empresa>> GetAllAsync(CancellationToken cancellationToken = default);
}
