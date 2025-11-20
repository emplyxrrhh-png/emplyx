namespace Emplyx.Blazor.Services.Features;

public interface IFeatureFlags
{
    bool IsEnabled(string feature);
    bool Employees { get; }
    bool Access { get; }
    bool Scheduler { get; }
    bool Alerts { get; }
    bool Security { get; }
}

public sealed class FeatureFlags : IFeatureFlags
{
    private readonly IReadOnlyDictionary<string, bool> _flags;

    public FeatureFlags(IConfiguration configuration)
    {
        _flags = configuration.GetSection("Features").GetChildren()
            .ToDictionary(c => c.Key, c => bool.TryParse(c.Value, out var b) && b, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsEnabled(string feature) => _flags.TryGetValue(feature, out var b) && b;

    public bool Employees => IsEnabled(nameof(Employees));
    public bool Access => IsEnabled(nameof(Access));
    public bool Scheduler => IsEnabled(nameof(Scheduler));
    public bool Alerts => IsEnabled(nameof(Alerts));
    public bool Security => IsEnabled(nameof(Security));
}
