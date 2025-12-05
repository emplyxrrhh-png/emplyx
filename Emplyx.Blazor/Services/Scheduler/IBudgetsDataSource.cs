using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public interface IBudgetsDataSource
{
    Task<BudgetListResponse> GetAsync(BudgetListRequest request, CancellationToken cancellationToken = default);
}
