namespace Emplyx.ApiAdapter.Contracts;

public sealed record ApiError(
    string? Code,
    string? Message,
    IReadOnlyDictionary<string, string[]>? Details);
