using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public sealed class AccessZonesMockDataSource : IAccessZonesDataSource
{
    private static readonly IReadOnlyList<AccessZoneListItem> Seed =
    [
        new("AZ-0001", "Lobby Principal", "room", "-", "Ingreso principal", 4, 6, true, DateTimeOffset.UtcNow.AddMinutes(-30)),
        new("AZ-0002", "Dock 3", "dock", "Operaciones Norte", "Carga pesados", 6, 8, true, DateTimeOffset.UtcNow.AddHours(-3)),
        new("AZ-0003", "Oficina RH", "room", "HQ", "Acceso restringido", 2, 2, false, DateTimeOffset.UtcNow.AddDays(-1)),
        new("AZ-0004", "Laboratorio B2", "room", "Laboratorio", "Bioseguridad alta", 3, 5, true, DateTimeOffset.UtcNow.AddHours(-6)),
        new("AZ-0005", "Estacionamiento", "area", "-", "Perimetro externo", 5, 4, false, DateTimeOffset.UtcNow.AddDays(-2))
    ];

    public Task<AccessZoneListResponse> GetAsync(AccessZoneListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<AccessZoneListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                (item.Description?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            query = query.Where(item => item.Type.Equals(request.Type, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Parent))
        {
            query = query.Where(item => item.ParentName.Contains(request.Parent, StringComparison.OrdinalIgnoreCase));
        }

        if (request.IsCritical.HasValue)
        {
            query = query.Where(item => item.IsCritical == request.IsCritical);
        }

        if (!string.IsNullOrWhiteSpace(request.SortField))
        {
            query = ApplySort(query, request.SortField!, request.SortDescending);
        }

        var total = query.Count();
        var items = query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Task.FromResult(new AccessZoneListResponse(items, total));
    }

    private static IEnumerable<AccessZoneListItem> ApplySort(IEnumerable<AccessZoneListItem> source, string field, bool desc) =>
        field.ToLowerInvariant() switch
        {
            "name" => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name),
            "type" => desc ? source.OrderByDescending(x => x.Type) : source.OrderBy(x => x.Type),
            "parentname" => desc ? source.OrderByDescending(x => x.ParentName) : source.OrderBy(x => x.ParentName),
            "doorscount" => desc ? source.OrderByDescending(x => x.DoorsCount) : source.OrderBy(x => x.DoorsCount),
            "sensorscount" => desc ? source.OrderByDescending(x => x.SensorsCount) : source.OrderBy(x => x.SensorsCount),
            "updatedat" => desc ? source.OrderByDescending(x => x.UpdatedAt) : source.OrderBy(x => x.UpdatedAt),
            _ => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name)
        };
}
