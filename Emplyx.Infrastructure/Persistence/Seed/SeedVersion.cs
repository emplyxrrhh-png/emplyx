namespace Emplyx.Infrastructure.Persistence.Seed;

internal sealed class SeedVersion
{
    public Guid Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public int Version { get; set; }

    public DateTime AppliedAtUtc { get; set; }
}
