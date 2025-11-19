namespace Emplyx.Shared.UI.Selectors;

public sealed record SelectorRequest(
    string? Search,
    int PageIndex = 0,
    int PageSize = 10);
