namespace Emplyx.Shared.UI.DataGrid;

public sealed record DataGridSortState(
    string? Field,
    bool Descending,
    Func<string, Task> SortAsync);
