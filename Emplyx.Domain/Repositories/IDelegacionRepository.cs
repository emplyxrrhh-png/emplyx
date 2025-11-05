using Emplyx.Domain.Entities.Delegaciones;

namespace Emplyx.Domain.Repositories;

public interface IDelegacionRepository : IRepository<Delegacion>
{
    Task<Delegacion?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Delegacion>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Delegacion>> GetAllAsync(CancellationToken cancellationToken = default);
}
