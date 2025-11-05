namespace Emplyx.Domain.Entities.Usuarios;

public sealed class UsuarioSesion : Emplyx.Domain.Abstractions.Entity
{
    private UsuarioSesion()
        : base(Guid.Empty)
    {
    }

    internal UsuarioSesion(Guid id, Guid usuarioId, string device, string? ipAddress, DateTime createdAtUtc, DateTime expiresAtUtc)
        : base(id)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("User id must be provided.", nameof(usuarioId));
        }

        if (string.IsNullOrWhiteSpace(device))
        {
            throw new ArgumentException("Device description must be provided.", nameof(device));
        }

        UsuarioId = usuarioId;
        Device = device.Trim();
        IpAddress = ipAddress?.Trim();
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        IsActive = true;
    }

    public Guid UsuarioId { get; private set; }

    public string Device { get; private set; } = string.Empty;

    public string? IpAddress { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime? ClosedAtUtc { get; private set; }

    public void Close()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        ClosedAtUtc = DateTime.UtcNow;
    }

    public void Extend(TimeSpan extension)
    {
        ExpiresAtUtc = ExpiresAtUtc.Add(extension);
    }
}
