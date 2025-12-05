using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Usuarios;

public sealed class Usuario : Entity, IAggregateRoot
{
    private readonly HashSet<UsuarioContexto> _contextos = new();
    private readonly HashSet<UsuarioLicencia> _licencias = new();
    private readonly HashSet<UsuarioSesion> _sesiones = new();
    private readonly HashSet<UsuarioRol> _roles = new();

    private Usuario()
    {
        Perfil = UsuarioPerfil.Empty;
    }

    public Usuario(Guid id, string userName, string email, string displayName, Guid? clearanceId = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("User name cannot be empty.", nameof(userName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        }

        UserName = userName.Trim();
        Email = email.Trim();
        DisplayName = displayName.Trim();
        ClearanceId = clearanceId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
        IsActive = true;
        Perfil = UsuarioPerfil.Empty;
    }

    public string UserName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public Guid? ClearanceId { get; private set; }

    public string? PasswordHash { get; private set; }

    public string? ExternalIdentityId { get; private set; }

    public Guid? PreferredContextoId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public DateTime? LastLoginAtUtc { get; private set; }

    public DateTime? LastPasswordChangeAtUtc { get; private set; }

    public UsuarioPerfil Perfil { get; private set; }

    public IReadOnlyCollection<UsuarioRol> Roles => _roles;

    public IReadOnlyCollection<UsuarioContexto> Contextos => _contextos;

    public IReadOnlyCollection<UsuarioLicencia> Licencias => _licencias;

    public IReadOnlyCollection<UsuarioSesion> Sesiones => _sesiones;

    public void UpdateProfile(string displayName, string email)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }

        DisplayName = displayName.Trim();
        Email = email.Trim();
        Touch();
    }

    public void UpdatePerfil(string? nombres, string? apellidos, string? departamento, string? cargo, string? telefono)
    {
        Perfil = UsuarioPerfil.Create(nombres, apellidos, departamento, cargo, telefono);
        Touch();
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        Touch();
    }

    public void RegisterLogin()
    {
        LastLoginAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void SetPasswordHash(string? passwordHash)
    {
        PasswordHash = string.IsNullOrWhiteSpace(passwordHash) ? null : passwordHash.Trim();
        LastPasswordChangeAtUtc = PasswordHash is null ? LastPasswordChangeAtUtc : DateTime.UtcNow;
        Touch();
    }

    public void SetExternalIdentity(string? externalIdentityId)
    {
        ExternalIdentityId = string.IsNullOrWhiteSpace(externalIdentityId) ? null : externalIdentityId.Trim();
        Touch();
    }

    public void SetPreferredContexto(Guid? contextoId)
    {
        if (contextoId.HasValue && contextoId.Value == Guid.Empty)
        {
            throw new ArgumentException("Context id must be provided.", nameof(contextoId));
        }

        if (contextoId.HasValue && _contextos.All(c => c.ContextoId != contextoId.Value))
        {
            throw new InvalidOperationException("The user must be linked to the context before setting it as preferred.");
        }

        PreferredContextoId = contextoId;
        Touch();
    }

    public void SetClearance(Guid clearanceId)
    {
        if (clearanceId == Guid.Empty)
        {
            throw new ArgumentException("Clearance id must be provided.", nameof(clearanceId));
        }

        ClearanceId = clearanceId;
        Touch();
    }

    public void ClearClearance()
    {
        if (!ClearanceId.HasValue)
        {
            return;
        }

        ClearanceId = null;
        Touch();
    }

    public bool AssignRol(Guid rolId, Guid? contextoId = null)
    {
        if (rolId == Guid.Empty)
        {
            throw new ArgumentException("Role id must be provided.", nameof(rolId));
        }

        var targetContextId = contextoId ?? Guid.Empty;

        if (_roles.Any(r => r.RolId == rolId && r.ContextoId == targetContextId))
        {
            return false;
        }

        _roles.Add(new UsuarioRol(Id, rolId, targetContextId, DateTime.UtcNow));
        Touch();
        return true;
    }

    public bool RemoveRol(Guid rolId, Guid? contextoId = null)
    {
        var targetContextId = contextoId ?? Guid.Empty;
        var rol = _roles.SingleOrDefault(r => r.RolId == rolId && r.ContextoId == targetContextId);
        if (rol is null)
        {
            return false;
        }

        _roles.Remove(rol);
        Touch();
        return true;
    }

    public bool AttachContexto(Guid contextoId, bool isPrimary = false)
    {
        if (contextoId == Guid.Empty)
        {
            throw new ArgumentException("Context id must be provided.", nameof(contextoId));
        }

        if (_contextos.Any(c => c.ContextoId == contextoId))
        {
            return false;
        }

        if (isPrimary)
        {
            foreach (var contexto in _contextos.Where(c => c.IsPrimary))
            {
                contexto.MarkAsSecondary();
            }

            PreferredContextoId = contextoId;
        }

        _contextos.Add(new UsuarioContexto(Id, contextoId, DateTime.UtcNow, isPrimary));
        Touch();
        return true;
    }

    public bool DetachContexto(Guid contextoId)
    {
        var contexto = _contextos.SingleOrDefault(c => c.ContextoId == contextoId);
        if (contexto is null)
        {
            return false;
        }

        _contextos.Remove(contexto);

        if (PreferredContextoId == contextoId)
        {
            PreferredContextoId = _contextos.FirstOrDefault(c => c.IsPrimary)?.ContextoId ?? _contextos.FirstOrDefault()?.ContextoId;
        }

        Touch();
        return true;
    }

    public bool SetPrimaryContexto(Guid contextoId)
    {
        if (!_contextos.Any(c => c.ContextoId == contextoId))
        {
            return false;
        }

        foreach (var contexto in _contextos)
        {
            contexto.SetPrimary(contexto.ContextoId == contextoId);
        }

        PreferredContextoId = contextoId;
        Touch();
        return true;
    }

    public void AssignLicencia(Guid licenciaId)
    {
        if (licenciaId == Guid.Empty)
        {
            throw new ArgumentException("License id must be provided.", nameof(licenciaId));
        }

        if (_licencias.Any(l => l.LicenciaId == licenciaId))
        {
            return;
        }

        _licencias.Add(new UsuarioLicencia(Id, licenciaId, DateTime.UtcNow));
        Touch();
    }

    public void RevokeLicencia(Guid licenciaId)
    {
        var licencia = _licencias.SingleOrDefault(l => l.LicenciaId == licenciaId);
        if (licencia is null)
        {
            return;
        }

        _licencias.Remove(licencia);
        Touch();
    }

    public UsuarioSesion RegisterSession(string device, string? ipAddress, TimeSpan lifetime)
    {
        if (string.IsNullOrWhiteSpace(device))
        {
            throw new ArgumentException("Device description must be provided.", nameof(device));
        }

        var session = new UsuarioSesion(Guid.NewGuid(), Id, device.Trim(), ipAddress, DateTime.UtcNow, DateTime.UtcNow.Add(lifetime));
        _sesiones.Add(session);
        Touch();
        return session;
    }

    public void CloseSession(Guid sessionId)
    {
        var session = _sesiones.SingleOrDefault(s => s.Id == sessionId);
        session?.Close();
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
