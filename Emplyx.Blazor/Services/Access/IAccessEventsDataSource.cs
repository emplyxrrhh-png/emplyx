using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public interface IAccessEventsDataSource
{
    Task<AccessEventListResponse> GetAsync(AccessEventListRequest request, CancellationToken cancellationToken = default);
}
