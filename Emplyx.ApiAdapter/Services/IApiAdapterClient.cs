using Emplyx.ApiAdapter.Results;

namespace Emplyx.ApiAdapter.Services;

public interface IApiAdapterClient
{
    Task<ApiResult<TResponse>> GetAsync<TResponse>(string uri, CancellationToken cancellationToken = default);

    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest payload, CancellationToken cancellationToken = default);
}
