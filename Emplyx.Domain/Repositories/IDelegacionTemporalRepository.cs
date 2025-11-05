using Emplyx.Domain.Entities.DelegacionesTemporales;

namespace Emplyx.Domain.Repositories;

public interface IDelegacionTemporalRepository : IRepository<DelegacionTemporal>
{
    Task<IReadOnlyCollection<DelegacionTemporal>> GetPendientesAsync(Guid delegadoId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DelegacionTemporal>> GetActivasAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}
