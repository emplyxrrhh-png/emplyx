namespace Emplyx.Shared.Scheduler;

public sealed record CoverageSummaryRequest(
    DateOnly From,
    DateOnly To,
    string? Unit = null,
    string? Status = null);
