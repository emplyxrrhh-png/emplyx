using Emplyx.Shared.UI.Tasks;

namespace Emplyx.Blazor.Services;

public sealed class SampleTasksQueueService
{
    private static readonly IReadOnlyList<TaskQueueItem> Seed =
    [
        new TaskQueueItem("Q-1001", "Alerts.Export", "completed", 100, DateTimeOffset.UtcNow.AddMinutes(-35), DateTimeOffset.UtcNow.AddMinutes(-5)),
        new TaskQueueItem("Q-1002", "Terminals.Broadcast", "running", 62, DateTimeOffset.UtcNow.AddMinutes(-12), null),
        new TaskQueueItem("Q-1003", "Employees.Sync", "failed", 15, DateTimeOffset.UtcNow.AddMinutes(-20), DateTimeOffset.UtcNow.AddMinutes(-15)),
        new TaskQueueItem("Q-1004", "DataLink.Import", "pending", 0, DateTimeOffset.UtcNow.AddMinutes(-2), null)
    ];

    public Task<IReadOnlyList<TaskQueueItem>> GetQueueAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<TaskQueueItem>>(Seed);
}
