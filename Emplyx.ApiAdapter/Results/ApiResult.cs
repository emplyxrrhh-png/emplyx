using System.Net;
using Emplyx.ApiAdapter.Contracts;

namespace Emplyx.ApiAdapter.Results;

public sealed record ApiResult<TResponse>(
    bool Success,
    TResponse? Data,
    ApiError? Error,
    HttpStatusCode StatusCode)
{
    public static ApiResult<TResponse> FromEnvelope(ApiEnvelope<TResponse> envelope, HttpStatusCode statusCode) =>
        new(envelope.Ok, envelope.Data, envelope.Error, statusCode);

    public static ApiResult<TResponse> Failure(ApiError? error, HttpStatusCode statusCode) =>
        new(false, default, error, statusCode);
}
