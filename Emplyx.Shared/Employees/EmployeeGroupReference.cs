namespace Emplyx.Shared.Employees;

/// <summary>
///     Referencia ligera a un grupo de Employees para reutilizar en listados y Selectors.
/// </summary>
public sealed record EmployeeGroupReference(
    string Id,
    string Name,
    string? Scope = null);
