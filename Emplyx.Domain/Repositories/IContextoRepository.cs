using Emplyx.Domain.Entities.Contextos;

namespace Emplyx.Domain.Repositories;

public interface IContextoRepository : IRepository<Contexto>
{
    Task<Contexto?> GetByClaveAsync(string clave, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Contexto>> GetByDelegacionAsync(Guid delegacionId, CancellationToken cancellationToken = default);
}
