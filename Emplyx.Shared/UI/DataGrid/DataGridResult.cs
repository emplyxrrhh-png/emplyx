namespace Emplyx.Shared.UI.DataGrid;

public sealed record DataGridResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount);
