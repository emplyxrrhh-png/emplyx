using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public sealed class AccessEventsMockDataSource : IAccessEventsDataSource
{
    private static readonly IReadOnlyList<AccessEventListItem> Seed =
    [
        new("AE-0001", "Broadcast HQ", "Lobby Principal", "HQ", DateTimeOffset.UtcNow.AddDays(1), "Programado", true, true),
        new("AE-0002", "Sincronizacion Dock", "Dock 3", "Operaciones Norte", DateTimeOffset.UtcNow.AddHours(12), "Programado", true, true),
        new("AE-0003", "Bloqueo laboratorio", "Laboratorio B2", "Laboratorio", DateTimeOffset.UtcNow.AddMinutes(-30), "Finalizado", false, false),
        new("AE-0004", "Habilitacion visitantes", "Sala Reunion 2", "Visitantes", DateTimeOffset.UtcNow.AddHours(4), "Programado", true, true),
        new("AE-0005", "Mantenimiento sensores", "Estacionamiento", "Operaciones", DateTimeOffset.UtcNow.AddDays(2), "Programado", true, true)
    ];

    public Task<AccessEventListResponse> GetAsync(AccessEventListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<AccessEventListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.ZoneName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Zone))
        {
            query = query.Where(item => item.ZoneName.Contains(request.Zone, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Group))
        {
            query = query.Where(item => item.GroupName.Contains(request.Group, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
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

        return Task.FromResult(new AccessEventListResponse(items, total));
    }

    private static IEnumerable<AccessEventListItem> ApplySort(IEnumerable<AccessEventListItem> source, string field, bool desc) =>
        field.ToLowerInvariant() switch
        {
            "name" => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name),
            "zonename" => desc ? source.OrderByDescending(x => x.ZoneName) : source.OrderBy(x => x.ZoneName),
            "groupname" => desc ? source.OrderByDescending(x => x.GroupName) : source.OrderBy(x => x.GroupName),
            "scheduleddate" => desc ? source.OrderByDescending(x => x.ScheduledDate) : source.OrderBy(x => x.ScheduledDate),
            "status" => desc ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            _ => desc ? source.OrderByDescending(x => x.ScheduledDate) : source.OrderBy(x => x.ScheduledDate)
        };
}
