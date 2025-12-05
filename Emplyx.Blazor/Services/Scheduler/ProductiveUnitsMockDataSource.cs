using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public sealed class ProductiveUnitsMockDataSource : IProductiveUnitsDataSource
{
    private static readonly IReadOnlyList<ProductiveUnitListItem> Seed =
    [
        new("PU-001", "Línea Ensamble HQ", "HQ", "Monica Rueda", "Activo", 4, 120, DateTimeOffset.UtcNow.AddHours(-3)),
        new("PU-002", "Planta Norte", "Operaciones Norte", "Carlos Vidal", "Activo", 6, 220, DateTimeOffset.UtcNow.AddHours(-6)),
        new("PU-003", "Visitantes", "HQ", "Laura Estrada", "Suspendido", 1, 25, DateTimeOffset.UtcNow.AddDays(-2)),
        new("PU-004", "Laboratorio B", "Laboratorio", "Daniel Ruiz", "Activo", 3, 45, DateTimeOffset.UtcNow.AddHours(-1)),
        new("PU-005", "Turno Nocturno", "Operaciones Norte", "Ana Morales", "Activo", 2, 60, DateTimeOffset.UtcNow.AddHours(-12))
    ];

    public Task<ProductiveUnitListResponse> GetAsync(ProductiveUnitListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProductiveUnitListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.Plant.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Plant))
        {
            query = query.Where(item => item.Plant.Contains(request.Plant, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Manager))
        {
            query = query.Where(item => item.Manager.Contains(request.Manager, StringComparison.OrdinalIgnoreCase));
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

        return Task.FromResult(new ProductiveUnitListResponse(items, total));
    }

    private static IEnumerable<ProductiveUnitListItem> ApplySort(IEnumerable<ProductiveUnitListItem> source, string field, bool desc) =>
        field.ToLowerInvariant() switch
        {
            "name" => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name),
            "plant" => desc ? source.OrderByDescending(x => x.Plant) : source.OrderBy(x => x.Plant),
            "manager" => desc ? source.OrderByDescending(x => x.Manager) : source.OrderBy(x => x.Manager),
            "status" => desc ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            "updatedat" => desc ? source.OrderByDescending(x => x.UpdatedAt) : source.OrderBy(x => x.UpdatedAt),
            _ => desc ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name)
        };
}
