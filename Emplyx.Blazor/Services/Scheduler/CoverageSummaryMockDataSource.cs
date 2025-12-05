using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public sealed class CoverageSummaryMockDataSource : ICoverageSummaryDataSource
{
    private static readonly IReadOnlyList<CoverageSummaryItem> Seed =
    [
        new CoverageSummaryItem("Línea Ensamble HQ", DateOnly.FromDateTime(DateTime.Today), 120m, 110m, 91.6m, 1, "Normal"),
        new CoverageSummaryItem("Planta Norte", DateOnly.FromDateTime(DateTime.Today), 180m, 150m, 83.3m, 4, "Atención"),
        new CoverageSummaryItem("Laboratorio B", DateOnly.FromDateTime(DateTime.Today), 60m, 58m, 96.7m, 0, "Normal"),
        new CoverageSummaryItem("Visitantes", DateOnly.FromDateTime(DateTime.Today), 20m, 10m, 50m, 2, "Crítico")
    ];

    public Task<IReadOnlyList<CoverageSummaryItem>> GetAsync(CoverageSummaryRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<CoverageSummaryItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Unit))
        {
            query = query.Where(item => item.UnitName.Contains(request.Unit, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (request.From != default || request.To != default)
        {
            query = query.Where(item => item.Day >= request.From && item.Day <= request.To);
        }

        return Task.FromResult<IReadOnlyList<CoverageSummaryItem>>(query.ToList());
    }
}
