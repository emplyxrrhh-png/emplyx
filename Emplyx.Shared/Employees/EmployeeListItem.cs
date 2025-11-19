namespace Emplyx.Shared.Employees;

/// <summary>
///     Item utilizado en el listado principal de Employees (Employees/Employees.aspx).
///     Incluye metadata m�nima para componer acciones y badges sin consultar el detalle.
/// </summary>
public sealed record EmployeeListItem(
    string Id,
    string Code,
    string DisplayName,
    string Status,
    string PrimaryRole,
    IReadOnlyList<EmployeeGroupReference> Groups,
    bool HasForgottenRight,
    string Language,
    bool Enabled);
