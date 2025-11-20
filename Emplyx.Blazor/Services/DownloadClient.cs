using System.Net.Http.Headers;
using Emplyx.Shared.UI.Telemetry;

namespace Emplyx.Blazor.Services;

public sealed class DownloadClient
{
    private readonly HttpClient _http;
    private readonly UiTelemetry _telemetry;

    public DownloadClient(HttpClient http, UiTelemetry telemetry)
    {
        _http = http;
        _telemetry = telemetry;
    }

    public sealed record DownloadResult(bool Ok, string? FileName, string? ContentType, byte[]? Bytes, string? ErrorCode, string CorrelationId);

    public async Task<DownloadResult> DownloadAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var correlation = _telemetry.NewCorrelationId();
        _telemetry.TrackDownloadStarted("file", endpoint, endpoint, correlation);
        try
        {
            using var response = await _http.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _telemetry.TrackDownloadCompleted("file", endpoint, endpoint, false, null, correlation, ((int)response.StatusCode).ToString());
                return new DownloadResult(false, null, null, null, ((int)response.StatusCode).ToString(), correlation);
            }
            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = ResolveFileName(response.Content.Headers.ContentDisposition, endpoint) ?? "download.bin";
            _telemetry.TrackDownloadCompleted("file", endpoint, endpoint, true, null, correlation);
            return new DownloadResult(true, fileName, contentType, bytes, null, correlation);
        }
        catch (OperationCanceledException)
        {
            _telemetry.TrackDownloadCompleted("file", endpoint, endpoint, false, null, correlation, "cancelled");
            return new DownloadResult(false, null, null, null, "cancelled", correlation);
        }
        catch (Exception ex)
        {
            _telemetry.TrackDownloadCompleted("file", endpoint, endpoint, false, null, correlation, ex.GetType().Name);
            return new DownloadResult(false, null, null, null, ex.GetType().Name, correlation);
        }
    }

    private static string? ResolveFileName(ContentDispositionHeaderValue? disposition, string fallback)
    {
        if (disposition?.FileNameStar is { Length: > 0 }) return disposition.FileNameStar.Trim('"');
        if (disposition?.FileName is { Length: > 0 }) return disposition.FileName.Trim('"');
        // fallback to last segment of endpoint
        try
        {
            var uri = new Uri(fallback, UriKind.RelativeOrAbsolute);
            var segment = uri.IsAbsoluteUri ? uri.Segments.LastOrDefault() : fallback.Split('/').LastOrDefault();
            return segment;
        }
        catch
        {
            return null;
        }
    }
}
