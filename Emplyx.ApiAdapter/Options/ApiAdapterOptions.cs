namespace Emplyx.ApiAdapter.Options;

public sealed class ApiAdapterOptions
{
    public string? BaseUrl { get; set; }

    public double TimeoutSeconds { get; set; } = 30;

    public Uri? ResolveBaseAddress()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            return null;
        }

        return Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) ? uri :
            throw new InvalidOperationException($"ApiAdapter.BaseUrl '{BaseUrl}' no es una URI valida.");
    }

    public TimeSpan ResolveTimeout()
    {
        if (TimeoutSeconds <= 0)
        {
            return TimeSpan.FromSeconds(30);
        }

        var capped = Math.Clamp(TimeoutSeconds, 5, 120);
        return TimeSpan.FromSeconds(capped);
    }
}
