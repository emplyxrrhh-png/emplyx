using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public interface IAccessZonesDataSource
{
    Task<AccessZoneListResponse> GetAsync(AccessZoneListRequest request, CancellationToken cancellationToken = default);
}
