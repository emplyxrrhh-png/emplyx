using Emplyx.Domain.Entities.Roles;

namespace Emplyx.Domain.Repositories;

public interface IRolRepository : IRepository<Rol>
{
    Task<Rol?> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Rol>> GetByPermisoAsync(Guid permisoId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Rol>> GetAllAsync(CancellationToken cancellationToken = default);
}
