namespace Emplyx.Shared.UI.Selectors;

public sealed record SelectorResult<T>(
    IReadOnlyList<T> Items,
    int Total);
