using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public sealed class AccessStatusMockDataSource : IAccessStatusDataSource
{
    private static readonly IReadOnlyList<AccessStatusListItem> Seed =
    [
        new("AS-0001", "HQ - Administracion", "Lobby", "Monica Rueda", "Autorizado", "info", DateTimeOffset.UtcNow.AddMinutes(-5), false),
        new("AS-0002", "Operaciones Norte", "Dock 3", "Carlos Vidal", "Denegado", "error", DateTimeOffset.UtcNow.AddMinutes(-1), true),
        new("AS-0003", "Visitantes", "Sala Reunion 2", "Laura Estrada", "Pendiente", "warning", DateTimeOffset.UtcNow.AddMinutes(-15), true),
        new("AS-0004", "Laboratorio", "Bioseguridad B2", "Daniel Ruiz", "Autorizado", "info", DateTimeOffset.UtcNow.AddMinutes(-2), false),
        new("AS-0005", "Almacen nocturno", "Zona Carga", "Ana Morales", "Denegado", "critical", DateTimeOffset.UtcNow.AddMinutes(-8), true)
    ];

    public Task<AccessStatusListResponse> GetAsync(AccessStatusListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<AccessStatusListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.EmployeeName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.ZoneName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.GroupName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Group))
        {
            query = query.Where(item => item.GroupName.Contains(request.Group, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Zone))
        {
            query = query.Where(item => item.ZoneName.Contains(request.Zone, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Severity))
        {
            query = query.Where(item => item.Severity.Equals(request.Severity, StringComparison.OrdinalIgnoreCase));
        }

        if (request.RequiresAttention.HasValue)
        {
            query = query.Where(item => item.RequiresAttention == request.RequiresAttention);
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

        return Task.FromResult(new AccessStatusListResponse(items, total));
    }

    private static IEnumerable<AccessStatusListItem> ApplySort(IEnumerable<AccessStatusListItem> source, string field, bool desc) =>
        field.ToLowerInvariant() switch
        {
            "groupname" => desc ? source.OrderByDescending(x => x.GroupName) : source.OrderBy(x => x.GroupName),
            "zonename" => desc ? source.OrderByDescending(x => x.ZoneName) : source.OrderBy(x => x.ZoneName),
            "employeename" => desc ? source.OrderByDescending(x => x.EmployeeName) : source.OrderBy(x => x.EmployeeName),
            "statusname" => desc ? source.OrderByDescending(x => x.StatusName) : source.OrderBy(x => x.StatusName),
            "severity" => desc ? source.OrderByDescending(x => x.Severity) : source.OrderBy(x => x.Severity),
            "lastevent" => desc ? source.OrderByDescending(x => x.LastEvent) : source.OrderBy(x => x.LastEvent),
            _ => desc ? source.OrderByDescending(x => x.LastEvent) : source.OrderBy(x => x.LastEvent)
        };
}
