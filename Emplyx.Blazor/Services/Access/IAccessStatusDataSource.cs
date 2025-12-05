using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public interface IAccessStatusDataSource
{
    Task<AccessStatusListResponse> GetAsync(AccessStatusListRequest request, CancellationToken cancellationToken = default);
}
