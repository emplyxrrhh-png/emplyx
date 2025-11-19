namespace Emplyx.Shared.UI.DataGrid;

public sealed record DataGridFilterDefinition(
    string Field,
    string Label,
    string? Placeholder = null);
