using Emplyx.Shared.Contracts.Usuarios;

namespace Emplyx.Application.Abstractions;

public interface IUsuarioService
{
    Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default);

    Task<UsuarioDto> UpdateAsync(UpdateUsuarioRequest request, CancellationToken cancellationToken = default);

    Task<UsuarioDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UsuarioDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UsuarioDto>> SearchAsync(SearchUsuariosRequest request, CancellationToken cancellationToken = default);
}
