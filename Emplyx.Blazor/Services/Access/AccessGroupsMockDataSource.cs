using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

/// <summary>
///     Proveedor temporal de Access Groups basado en el inventario 11.1.
///     Simula las respuestas de `srvAccessGroups.ashx` convertidas a JSON.
/// </summary>
public sealed class AccessGroupsMockDataSource : IAccessGroupsDataSource
{
    private static readonly IReadOnlyList<AccessGroupListItem> Seed =
    [
        new(
            Id: "AG-0001",
            Name: "HQ - Administracion",
            Description: "Acceso completo a oficinas centrales.",
            Status: "Activo",
            ZonesCount: 6,
            PeriodsCount: 4,
            EmployeesCount: 128,
            UpdatedAt: DateTimeOffset.UtcNow.AddDays(-2),
            HasPendingChanges: false),
        new(
            Id: "AG-0002",
            Name: "Operaciones Norte",
            Description: "Zonas industriales y docks.",
            Status: "Activo",
            ZonesCount: 8,
            PeriodsCount: 6,
            EmployeesCount: 240,
            UpdatedAt: DateTimeOffset.UtcNow.AddDays(-1),
            HasPendingChanges: true),
        new(
            Id: "AG-0003",
            Name: "Visitantes",
            Description: "Acceso restringido lobby + salas reunion.",
            Status: "Suspendido",
            ZonesCount: 3,
            PeriodsCount: 2,
            EmployeesCount: 45,
            UpdatedAt: DateTimeOffset.UtcNow.AddDays(-10),
            HasPendingChanges: false),
        new(
            Id: "AG-0004",
            Name: "Laboratorio",
            Description: "Zonas de bioseguridad b1-b2.",
            Status: "Activo",
            ZonesCount: 5,
            PeriodsCount: 5,
            EmployeesCount: 62,
            UpdatedAt: DateTimeOffset.UtcNow.AddHours(-6),
            HasPendingChanges: true),
        new(
            Id: "AG-0005",
            Name: "Almacen nocturno",
            Description: "Periodos especiales 22:00-06:00.",
            Status: "Activo",
            ZonesCount: 4,
            PeriodsCount: 3,
            EmployeesCount: 38,
            UpdatedAt: DateTimeOffset.UtcNow.AddDays(-4),
            HasPendingChanges: false),
        new(
            Id: "AG-0006",
            Name: "Mantencion externa",
            Description: "Contratistas + escoltas.",
            Status: "Inactivo",
            ZonesCount: 2,
            PeriodsCount: 1,
            EmployeesCount: 12,
            UpdatedAt: DateTimeOffset.UtcNow.AddDays(-30),
            HasPendingChanges: false)
    ];

    public Task<AccessGroupListResponse> GetAsync(AccessGroupListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<AccessGroupListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                (item.Description?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Zone))
        {
            query = query.Where(item => item.Description?.Contains(request.Zone, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (!string.IsNullOrWhiteSpace(request.Period))
        {
            query = query.Where(item => item.Description?.Contains(request.Period, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (request.HasPendingChanges.HasValue)
        {
            query = query.Where(item => item.HasPendingChanges == request.HasPendingChanges);
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

        return Task.FromResult(new AccessGroupListResponse(items, total));
    }

    public Task<IReadOnlyList<AccessGroupLookupItem>> SearchAsync(string? term, CancellationToken cancellationToken = default)
    {
        var query = Seed.Select(item => new AccessGroupLookupItem(
            Id: item.Id,
            Name: item.Name,
            Zone: $"{item.ZonesCount} zonas",
            Period: $"{item.PeriodsCount} periodos",
            Status: item.Status));

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(item => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult<IReadOnlyList<AccessGroupLookupItem>>(query.Take(20).ToList());
    }

    private static IEnumerable<AccessGroupListItem> ApplySort(IEnumerable<AccessGroupListItem> source, string field, bool desc)
    {
        return field.ToLowerInvariant() switch
        {
            "name" => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name),
            "status" => desc ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            "zonescount" => desc ? source.OrderByDescending(x => x.ZonesCount) : source.OrderBy(x => x.ZonesCount),
            "periodscount" => desc ? source.OrderByDescending(x => x.PeriodsCount) : source.OrderBy(x => x.PeriodsCount),
            "employeescount" => desc ? source.OrderByDescending(x => x.EmployeesCount) : source.OrderBy(x => x.EmployeesCount),
            "updatedat" => desc ? source.OrderByDescending(x => x.UpdatedAt) : source.OrderBy(x => x.UpdatedAt),
            _ => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name)
        };
    }
}
