using Emplyx.Shared.Contracts.Delegaciones;

namespace Emplyx.Application.Abstractions;

public interface IDelegacionService
{
    Task<DelegacionDto> UpsertAsync(UpsertDelegacionRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DelegacionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DelegacionDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
