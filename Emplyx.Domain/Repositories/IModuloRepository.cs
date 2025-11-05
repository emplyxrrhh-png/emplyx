using Emplyx.Domain.Entities.Modulos;

namespace Emplyx.Domain.Repositories;

public interface IModuloRepository : IRepository<Modulo>
{
    Task<Modulo?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Modulo>> GetAllAsync(CancellationToken cancellationToken = default);
}
