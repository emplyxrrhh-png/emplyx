using System.Globalization;

namespace Emplyx.Shared.UI;

/// <summary>
/// Representa el contexto multi-tenant que debe conocer la UI para seleccionar datos y formateos.
/// </summary>
public sealed record TenantContext(
    string TenantId,
    string? CompanyId,
    string? UnitId,
    string Culture,
    string TimeZone,
    string? UserId = null,
    string? License = null)
{
    public static TenantContext Default { get; } = new(
        TenantId: "default",
        CompanyId: null,
        UnitId: null,
        Culture: CultureInfo.CurrentCulture.Name,
        TimeZone: TimeZoneInfo.Local.Id);
}

public interface ITenantContextAccessor
{
    TenantContext Current { get; }

    event Action<TenantContext>? ContextChanged;

    Task SetAsync(TenantContext context, CancellationToken cancellationToken = default);
}
