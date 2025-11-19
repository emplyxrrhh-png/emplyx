using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public interface IMovesDataSource
{
    Task<DailyMoveListResponse> GetAsync(DailyMoveListRequest request, CancellationToken cancellationToken = default);
}
