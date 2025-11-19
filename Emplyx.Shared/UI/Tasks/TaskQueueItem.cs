namespace Emplyx.Shared.UI.Tasks;

public sealed record TaskQueueItem(
    string Id,
    string Type,
    string Status,
    int Progress,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt);
