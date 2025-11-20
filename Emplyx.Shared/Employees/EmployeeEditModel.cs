using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Emplyx.Shared.Employees;

public sealed class EmployeeEditModel
{
    [Required(ErrorMessage = "Employees.Form.Error.NameRequired")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Employees.Form.Error.NameLength")]
    public string? DisplayName { get; set; }

    [Required(ErrorMessage = "Employees.Form.Error.LanguageRequired")]
    [RegularExpression("^(es-ES|en-US)$", ErrorMessage = "Employees.Form.Error.LanguageUnsupported")] // allowed for v0.1
    public string? Language { get; set; } = CultureInfo.CurrentCulture.Name switch
    {
        "es-ES" => "es-ES",
        _ => "en-US"
    };

    [StringLength(10, ErrorMessage = "Employees.Form.Error.CodeLength")]
    public string? Code { get; set; }

    public bool ForgottenRight { get; set; }
}
