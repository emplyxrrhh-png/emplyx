using Emplyx.Domain.Entities.Usuarios;

namespace Emplyx.Domain.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> HasPermissionAsync(Guid userId, string permission, Guid? contextoId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Usuario>> SearchAsync(
        string? userNameOrEmail,
        Guid? contextoId,
        Guid? rolId,
        CancellationToken cancellationToken = default);
}
