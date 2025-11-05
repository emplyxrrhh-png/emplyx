using Emplyx.Domain.Entities.Clearances;

namespace Emplyx.Domain.Repositories;

public interface IClearanceRepository : IRepository<Clearance>
{
    Task<Clearance?> GetByNivelAsync(int nivel, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Clearance>> GetActivasAsync(CancellationToken cancellationToken = default);
}
