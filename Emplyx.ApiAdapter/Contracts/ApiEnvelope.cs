namespace Emplyx.ApiAdapter.Contracts;

public sealed record ApiEnvelope<T>(
    bool Ok,
    T? Data,
    object? Meta,
    ApiError? Error);
