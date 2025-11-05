using Emplyx.Shared.Contracts.Roles;

namespace Emplyx.Application.Abstractions;

public interface IRolService
{
    Task<RolDto> UpsertAsync(UpsertRolRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RolDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RolDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
