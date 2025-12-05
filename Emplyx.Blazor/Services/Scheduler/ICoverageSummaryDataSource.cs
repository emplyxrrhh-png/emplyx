using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public interface ICoverageSummaryDataSource
{
    Task<IReadOnlyList<CoverageSummaryItem>> GetAsync(CoverageSummaryRequest request, CancellationToken cancellationToken = default);
}
