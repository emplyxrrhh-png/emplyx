namespace Emplyx.Shared.Scheduler;

public sealed record CoverageSummaryItem(
    string UnitName,
    DateOnly Day,
    decimal PlannedHours,
    decimal WorkedHours,
    decimal CoveragePercent,
    int Incidents,
    string Status);
