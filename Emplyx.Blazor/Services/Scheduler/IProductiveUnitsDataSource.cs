using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public interface IProductiveUnitsDataSource
{
    Task<ProductiveUnitListResponse> GetAsync(ProductiveUnitListRequest request, CancellationToken cancellationToken = default);
}
