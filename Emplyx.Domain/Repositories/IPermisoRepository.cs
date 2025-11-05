using Emplyx.Domain.Entities.Permisos;

namespace Emplyx.Domain.Repositories;

public interface IPermisoRepository : IRepository<Permiso>
{
    Task<Permiso?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permiso>> GetByModuloAsync(Guid moduloId, CancellationToken cancellationToken = default);
}
