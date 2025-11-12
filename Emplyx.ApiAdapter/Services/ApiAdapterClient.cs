using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Emplyx.ApiAdapter.Contracts;
using Emplyx.ApiAdapter.Options;
using Emplyx.ApiAdapter.Results;
using Emplyx.ApiAdapter.Serialization;
using Emplyx.Shared.UI;
using Microsoft.Extensions.Options;

namespace Emplyx.ApiAdapter.Services;

public sealed class ApiAdapterClient : IApiAdapterClient
{
    private readonly HttpClient _httpClient;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly IOptionsMonitor<ApiAdapterOptions> _optionsMonitor;

    public ApiAdapterClient(
        HttpClient httpClient,
        ITenantContextAccessor tenantContextAccessor,
        IOptionsMonitor<ApiAdapterOptions> optionsMonitor)
    {
        _httpClient = httpClient;
        _tenantContextAccessor = tenantContextAccessor;
        _optionsMonitor = optionsMonitor;
    }

    public Task<ApiResult<TResponse>> GetAsync<TResponse>(string uri, CancellationToken cancellationToken = default) =>
        SendAsync<TResponse>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest payload, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(payload, options: SerializerDefaults.Options)
        };

        return SendAsync<TResponse>(request, cancellationToken);
    }

    private async Task<ApiResult<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ApplyTenantHeaders(request);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(body))
        {
            return ApiResult<TResponse>.Failure(
                new ApiError("empty_response", "El adaptador devolvio una respuesta vacia.", null),
                response.StatusCode);
        }

        try
        {
            var envelope = JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(body, SerializerDefaults.Options);
            if (envelope is null)
            {
                return ApiResult<TResponse>.Failure(
                    new ApiError("invalid_envelope", "No se pudo deserializar la respuesta del adaptador.", null),
                    response.StatusCode);
            }

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<TResponse>.Failure(
                    envelope.Error ?? new ApiError(response.StatusCode.ToString(), envelope.Error?.Message ?? "Error del adaptador.", null),
                    response.StatusCode);
            }

            return ApiResult<TResponse>.FromEnvelope(envelope, response.StatusCode);
        }
        catch (JsonException jsonEx)
        {
            return ApiResult<TResponse>.Failure(
                new ApiError("json_error", jsonEx.Message, null),
                response.StatusCode);
        }
    }

    private void ApplyTenantHeaders(HttpRequestMessage request)
    {
        var context = _tenantContextAccessor.Current ?? TenantContext.Default;
        request.Headers.Remove("X-Emplyx-Tenant");
        request.Headers.Remove("X-Emplyx-Company");
        request.Headers.Remove("X-Emplyx-Unit");
        request.Headers.Remove("X-Emplyx-Culture");
        request.Headers.Remove("X-Emplyx-TimeZone");

        request.Headers.TryAddWithoutValidation("X-Emplyx-Tenant", context.TenantId);

        if (!string.IsNullOrWhiteSpace(context.CompanyId))
        {
            request.Headers.TryAddWithoutValidation("X-Emplyx-Company", context.CompanyId);
        }

        if (!string.IsNullOrWhiteSpace(context.UnitId))
        {
            request.Headers.TryAddWithoutValidation("X-Emplyx-Unit", context.UnitId);
        }

        request.Headers.TryAddWithoutValidation("X-Emplyx-Culture", context.Culture);
        request.Headers.TryAddWithoutValidation("X-Emplyx-TimeZone", context.TimeZone);

        var options = _optionsMonitor.CurrentValue;
        if (options.ResolveBaseAddress() is { } baseAddress && _httpClient.BaseAddress is null)
        {
            _httpClient.BaseAddress = baseAddress;
            _httpClient.Timeout = options.ResolveTimeout();
        }
    }
}
