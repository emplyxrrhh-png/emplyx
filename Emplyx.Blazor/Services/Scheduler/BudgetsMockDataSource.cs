using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public sealed class BudgetsMockDataSource : IBudgetsDataSource
{
    private static readonly IReadOnlyList<BudgetListItem> Seed =
    [
        new("BG-001", "Línea Ensamble HQ", "Q1 2025", "Aprobado", 1200m, 450m, DateTimeOffset.UtcNow.AddDays(-1)),
        new("BG-002", "Planta Norte", "Q1 2025", "Pendiente", 1800m, 300m, DateTimeOffset.UtcNow.AddHours(-12)),
        new("BG-003", "Laboratorio B", "Q2 2025", "Borrador", 600m, 0m, DateTimeOffset.UtcNow.AddDays(-3)),
        new("BG-004", "Turno Nocturno", "Q1 2025", "Aprobado", 900m, 200m, DateTimeOffset.UtcNow.AddHours(-6)),
        new("BG-005", "Visitantes", "Q1 2025", "Suspendido", 150m, 20m, DateTimeOffset.UtcNow.AddDays(-10))
    ];

    public Task<BudgetListResponse> GetAsync(BudgetListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<BudgetListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.UnitName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.PeriodLabel.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Unit))
        {
            query = query.Where(item => item.UnitName.Contains(request.Unit, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Period))
        {
            query = query.Where(item => item.PeriodLabel.Contains(request.Period, StringComparison.OrdinalIgnoreCase));
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

        return Task.FromResult(new BudgetListResponse(items, total));
    }

    private static IEnumerable<BudgetListItem> ApplySort(IEnumerable<BudgetListItem> source, string field, bool desc) =>
        field.ToLowerInvariant() switch
        {
            "unitname" => desc ? source.OrderByDescending(x => x.UnitName) : source.OrderBy(x => x.UnitName),
            "periodlabel" => desc ? source.OrderByDescending(x => x.PeriodLabel) : source.OrderBy(x => x.PeriodLabel),
            "status" => desc ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            "plannedhours" => desc ? source.OrderByDescending(x => x.PlannedHours) : source.OrderBy(x => x.PlannedHours),
            "actualhours" => desc ? source.OrderByDescending(x => x.ActualHours) : source.OrderBy(x => x.ActualHours),
            "updatedat" => desc ? source.OrderByDescending(x => x.UpdatedAt) : source.OrderBy(x => x.UpdatedAt),
            _ => desc ? source.OrderByDescending(x => x.UnitName) : source.OrderBy(x => x.UnitName)
        };
}
