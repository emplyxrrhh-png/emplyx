using Emplyx.Domain.Entities.Licencias;

namespace Emplyx.Domain.Repositories;

public interface ILicenciaRepository : IRepository<Licencia>
{
    Task<Licencia?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Licencia>> GetExpiringWithinAsync(TimeSpan window, CancellationToken cancellationToken = default);
}
