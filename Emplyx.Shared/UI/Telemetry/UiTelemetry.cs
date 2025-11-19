using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Emplyx.Shared.UI.Telemetry;

/// <summary>
/// Servicio mínimo para registrar eventos de interacción UI (v0). Se ampliará con correlación y métricas.
/// </summary>
public sealed class UiTelemetry
{
    private readonly TelemetryClient _client;

    public UiTelemetry(TelemetryClient client)
    {
        _client = client;
    }

    public void TrackViewOpened(string page, string? correlationId = null)
        => TrackEvent("view.opened", page, correlationId);

    public void TrackActionClicked(string action, string page, string? correlationId = null, bool ok = true, string? errorCode = null)
        => TrackEvent("action.clicked", page, correlationId, new Dictionary<string,string>
        {
            ["action"] = action,
            ["ok"] = ok.ToString(),
            ["errorCode"] = errorCode ?? string.Empty
        });

    public void TrackDataLoad(string entity, string page, bool ok, long? durationMs = null, string? correlationId = null, string? errorCode = null)
        => TrackEvent("data.load", page, correlationId, new Dictionary<string,string>
        {
            ["entity"] = entity,
            ["ok"] = ok.ToString(),
            ["durationMs"] = durationMs?.ToString() ?? string.Empty,
            ["errorCode"] = errorCode ?? string.Empty
        });

    private void TrackEvent(string name, string page, string? correlationId, IDictionary<string,string>? extra = null)
    {
        var ev = new EventTelemetry(name);
        ev.Properties["page"] = page;
        if (!string.IsNullOrWhiteSpace(correlationId))
            ev.Properties["correlationId"] = correlationId;
        if (extra != null)
        {
            foreach (var kv in extra)
                ev.Properties[kv.Key] = kv.Value;
        }
        _client.TrackEvent(ev);
    }
}
