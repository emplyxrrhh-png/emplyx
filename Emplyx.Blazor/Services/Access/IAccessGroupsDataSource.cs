using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public interface IAccessGroupsDataSource
{
    Task<AccessGroupListResponse> GetAsync(AccessGroupListRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccessGroupLookupItem>> SearchAsync(string? term, CancellationToken cancellationToken = default);
}
