using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public interface IAccessPeriodsDataSource
{
    Task<AccessPeriodListResponse> GetAsync(AccessPeriodListRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccessPeriodLookupItem>> SearchAsync(string? term, CancellationToken cancellationToken = default);
}
