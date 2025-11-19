namespace Emplyx.Shared.Employees;

/// <summary>
///     Representaci�n ligera utilizada en Selectors/AppSelector.
/// </summary>
public sealed record EmployeeLookupItem(
    string Id,
    string Code,
    string DisplayName,
    string? Module = null,
    string? Status = null);
