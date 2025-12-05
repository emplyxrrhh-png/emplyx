using Emplyx.Application.Abstractions;
using Emplyx.Application.Mappers;
using Emplyx.Domain.Entities.Usuarios;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Shared.Contracts.Usuarios;

namespace Emplyx.Application.Services;

internal sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IContextoRepository _contextoRepository;
    private readonly ILicenciaRepository _licenciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IContextoRepository contextoRepository,
        ILicenciaRepository licenciaRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _contextoRepository = contextoRepository;
        _licenciaRepository = licenciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserDoesNotExist(request, cancellationToken);

        var usuario = new Usuario(Guid.NewGuid(), request.UserName, request.Email, request.DisplayName, request.ClearanceId);

        ApplyOptionalProfileData(usuario, request.Perfil);
        usuario.SetPasswordHash(request.PasswordHash);
        usuario.SetExternalIdentity(request.ExternalIdentityId);

        await AttachRolesAsync(usuario, request.Roles, cancellationToken);
        await AttachContextosAsync(usuario, request.Contextos, cancellationToken);
        await AttachLicenciasAsync(usuario, request.Licencias, cancellationToken);

        usuario.SetPreferredContexto(request.PreferredContextoId ?? usuario.Contextos.FirstOrDefault(c => c.IsPrimary)?.ContextoId);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return usuario.ToDto();
    }

    public async Task<UsuarioDto> UpdateAsync(UpdateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, cancellationToken)
                       ?? throw new InvalidOperationException("El usuario especificado no existe.");

        usuario.UpdateProfile(request.DisplayName, request.Email);
        ApplyOptionalProfileData(usuario, request.Perfil);

        usuario.SetPasswordHash(request.PasswordHash);
        usuario.SetExternalIdentity(request.ExternalIdentityId);

        if (request.ClearanceId.HasValue)
        {
            usuario.SetClearance(request.ClearanceId.Value);
        }
        else
        {
            usuario.ClearClearance();
        }

        if (request.IsActive)
        {
            usuario.Activate();
        }
        else
        {
            usuario.Deactivate();
        }

        await SyncRolesAsync(usuario, request.Roles, cancellationToken);
        await SyncContextosAsync(usuario, request.Contextos, cancellationToken);
        await SyncLicenciasAsync(usuario, request.Licencias, cancellationToken);

        usuario.SetPreferredContexto(request.PreferredContextoId ?? usuario.Contextos.FirstOrDefault(c => c.IsPrimary)?.ContextoId);

        _usuarioRepository.Update(usuario);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return usuario.ToDto();
    }

    public async Task<UsuarioDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        return usuario?.ToDto();
    }

    public async Task<IReadOnlyCollection<UsuarioDto>> SearchAsync(SearchUsuariosRequest request, CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.SearchAsync(
            request.UserNameOrEmail,
            request.ContextoId,
            request.RolId,
            cancellationToken);

        return usuarios.Select(u => u.ToDto()).ToArray();
    }

    private async Task EnsureUserDoesNotExist(CreateUsuarioRequest request, CancellationToken cancellationToken)
    {
        if (await _usuarioRepository.GetByUserNameAsync(request.UserName, cancellationToken) is not null)
        {
            throw new InvalidOperationException($"Ya existe un usuario con el nombre {request.UserName}.");
        }

        if (await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            throw new InvalidOperationException($"Ya existe un usuario con el correo {request.Email}.");
        }
    }

    private async Task AttachRolesAsync(Usuario usuario, IEnumerable<UsuarioRolAssignmentDto> roles, CancellationToken cancellationToken)
    {
        var distinctRoles = (roles ?? Array.Empty<UsuarioRolAssignmentDto>()).DistinctBy(r => new { r.RolId, r.ContextoId });
        foreach (var assignment in distinctRoles)
        {
            var rol = await _rolRepository.GetByIdAsync(assignment.RolId, cancellationToken);
            if (rol is null)
            {
                throw new InvalidOperationException($"El rol {assignment.RolId} no existe.");
            }

            usuario.AssignRol(assignment.RolId, assignment.ContextoId);
        }
    }

    private async Task AttachContextosAsync(Usuario usuario, IEnumerable<UsuarioContextoAssignmentDto> contextos, CancellationToken cancellationToken)
    {
        var distinctContextos = (contextos ?? Array.Empty<UsuarioContextoAssignmentDto>()).DistinctBy(c => c.ContextoId);
        foreach (var assignment in distinctContextos)
        {
            var contexto = await _contextoRepository.GetByIdAsync(assignment.ContextoId, cancellationToken);
            if (contexto is null)
            {
                throw new InvalidOperationException($"El contexto {assignment.ContextoId} no existe.");
            }

            usuario.AttachContexto(assignment.ContextoId, assignment.IsPrimary);
        }

        if (!usuario.Contextos.Any(c => c.IsPrimary) && usuario.Contextos.Any())
        {
            usuario.SetPrimaryContexto(usuario.Contextos.First().ContextoId);
        }
    }

    private async Task AttachLicenciasAsync(Usuario usuario, IEnumerable<Guid> licencias, CancellationToken cancellationToken)
    {
        var distinctLicencias = (licencias ?? Array.Empty<Guid>()).Distinct();
        foreach (var licenciaId in distinctLicencias)
        {
            var licencia = await _licenciaRepository.GetByIdAsync(licenciaId, cancellationToken);
            if (licencia is null)
            {
                throw new InvalidOperationException($"La licencia {licenciaId} no existe.");
            }

            usuario.AssignLicencia(licenciaId);
        }
    }

    private async Task SyncRolesAsync(Usuario usuario, IEnumerable<UsuarioRolAssignmentDto> roles, CancellationToken cancellationToken)
    {
        var desiredRoles = (roles ?? Array.Empty<UsuarioRolAssignmentDto>()).DistinctBy(r => new { r.RolId, r.ContextoId }).ToList();
        
        // Remove roles that are not in the desired list
        var currentRoles = usuario.Roles.ToList();
        foreach (var current in currentRoles)
        {
            if (!desiredRoles.Any(d => d.RolId == current.RolId && d.ContextoId == current.ContextoId))
            {
                usuario.RemoveRol(current.RolId, current.ContextoId);
            }
        }

        // Add new roles
        foreach (var desired in desiredRoles)
        {
            if (!usuario.Roles.Any(r => r.RolId == desired.RolId && r.ContextoId == (desired.ContextoId ?? Guid.Empty)))
            {
                var rol = await _rolRepository.GetByIdAsync(desired.RolId, cancellationToken);
                if (rol is null)
                {
                    throw new InvalidOperationException($"El rol {desired.RolId} no existe.");
                }

                usuario.AssignRol(desired.RolId, desired.ContextoId);
            }
        }
    }

    private async Task SyncContextosAsync(Usuario usuario, IEnumerable<UsuarioContextoAssignmentDto> contextos, CancellationToken cancellationToken)
    {
        var desired = (contextos ?? Array.Empty<UsuarioContextoAssignmentDto>()).DistinctBy(c => c.ContextoId).ToArray();
        var desiredIds = desired.Select(c => c.ContextoId).ToHashSet();
        var currentIds = usuario.Contextos.Select(c => c.ContextoId).ToHashSet();

        foreach (var toRemove in currentIds.Except(desiredIds).ToArray())
        {
            usuario.DetachContexto(toRemove);
        }

        foreach (var toAdd in desired.Where(c => !currentIds.Contains(c.ContextoId)))
        {
            var contexto = await _contextoRepository.GetByIdAsync(toAdd.ContextoId, cancellationToken);
            if (contexto is null)
            {
                throw new InvalidOperationException($"El contexto {toAdd.ContextoId} no existe.");
            }

            usuario.AttachContexto(toAdd.ContextoId, toAdd.IsPrimary);
        }

        var primary = desired.SingleOrDefault(c => c.IsPrimary);
        if (primary != default && desiredIds.Contains(primary.ContextoId))
        {
            usuario.SetPrimaryContexto(primary.ContextoId);
        }
    }

    private async Task SyncLicenciasAsync(Usuario usuario, IEnumerable<Guid> licencias, CancellationToken cancellationToken)
    {
        var desired = (licencias ?? Array.Empty<Guid>()).Distinct().ToHashSet();
        var current = usuario.Licencias.Select(l => l.LicenciaId).ToHashSet();

        foreach (var toRemove in current.Except(desired).ToArray())
        {
            usuario.RevokeLicencia(toRemove);
        }

        foreach (var toAdd in desired.Except(current))
        {
            var licencia = await _licenciaRepository.GetByIdAsync(toAdd, cancellationToken);
            if (licencia is null)
            {
                throw new InvalidOperationException($"La licencia {toAdd} no existe.");
            }

            usuario.AssignLicencia(toAdd);
        }
    }

    private static void ApplyOptionalProfileData(Usuario usuario, UsuarioPerfilDto? perfil)
    {
        perfil ??= new UsuarioPerfilDto(null, null, null, null, null);
        usuario.UpdatePerfil(perfil.Nombres, perfil.Apellidos, perfil.Departamento, perfil.Cargo, perfil.Telefono);
    }
}
