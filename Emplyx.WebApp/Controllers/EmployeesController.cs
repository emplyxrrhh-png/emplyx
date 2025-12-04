using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.Employees;
using Microsoft.AspNetCore.Mvc;

namespace Emplyx.WebApp.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IFileStorageService _fileStorage;

    public EmployeesController(IEmployeeService service, IFileStorageService fileStorage)
    {
        _service = service;
        _fileStorage = fileStorage;
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        using var stream = file.OpenReadStream();
        var url = await _fileStorage.UploadFileAsync(stream, fileName, file.ContentType);

        return Ok(new { url });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? empresaId, [FromQuery] Guid? centroTrabajoId)
    {
        if (centroTrabajoId.HasValue)
        {
            var result = await _service.GetByCentroTrabajoIdAsync(centroTrabajoId.Value);
            return Ok(result);
        }

        if (empresaId.HasValue)
        {
            var result = await _service.GetByEmpresaIdAsync(empresaId.Value);
            return Ok(result);
        }

        return BadRequest("Either empresaId or centroTrabajoId must be provided.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var employee = new Employee(
            Guid.NewGuid(),
            request.EmpresaId,
            request.CentroTrabajoId,
            request.Nombre,
            request.Apellidos,
            request.Alias,
            request.GroupName,
            request.Type,
            request.Status,
            request.Image
        );

        employee.UpdateDetails(
            request.Nombre,
            request.Apellidos,
            request.Alias,
            request.GroupName,
            request.Type,
            request.Status,
            request.Image,
            request.ContractType,
            request.StartDate,
            request.Idioma,
            request.RemoteWork,
            request.Notes
        );

        employee.UpdateControl(
            request.IDAccessGroup,
            request.BiometricID,
            request.AttControlled,
            request.AccControlled,
            request.JobControlled,
            request.ExtControlled,
            request.RiskControlled
        );

        employee.UpdateCredentials(
            request.WebLogin,
            request.WebPassword,
            request.ActiveDirectory
        );

        if (request.UserFields != null)
        {
            foreach (var field in request.UserFields)
            {
                employee.SetUserField(field.FieldDefinitionId, field.Value);
            }
        }

        var created = await _service.CreateAsync(employee);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        var employee = await _service.GetByIdAsync(id);
        if (employee == null) return NotFound();

        employee.UpdateDetails(
            request.Nombre,
            request.Apellidos,
            request.Alias,
            request.GroupName,
            request.Type,
            request.Status,
            request.Image,
            request.ContractType,
            request.StartDate,
            request.Idioma,
            request.RemoteWork,
            request.Notes
        );

        employee.UpdateControl(
            request.IDAccessGroup,
            request.BiometricID,
            request.AttControlled,
            request.AccControlled,
            request.JobControlled,
            request.ExtControlled,
            request.RiskControlled
        );

        employee.UpdateCredentials(
            request.WebLogin,
            request.WebPassword,
            request.ActiveDirectory
        );
        
        if (request.UserFields != null)
        {
             foreach (var field in request.UserFields)
            {
                employee.SetUserField(field.FieldDefinitionId, field.Value);
            }
        }

        await _service.UpdateAsync(employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

public record CreateEmployeeRequest(
    Guid EmpresaId,
    Guid? CentroTrabajoId,
    string Nombre,
    string Apellidos,
    string Alias,
    string GroupName,
    string Type,
    string Status,
    string? Image,
    string? ContractType,
    DateTime? StartDate,
    string? Idioma,
    bool RemoteWork,
    string? Notes,
    string? IDAccessGroup,
    string? BiometricID,
    bool AttControlled,
    bool AccControlled,
    bool JobControlled,
    bool ExtControlled,
    bool RiskControlled,
    string? WebLogin,
    string? WebPassword,
    bool ActiveDirectory,
    List<UserFieldDto>? UserFields
);

public record UpdateEmployeeRequest(
    string Nombre,
    string Apellidos,
    string Alias,
    string GroupName,
    string Type,
    string Status,
    string? Image,
    string? ContractType,
    DateTime? StartDate,
    string? Idioma,
    bool RemoteWork,
    string? Notes,
    string? IDAccessGroup,
    string? BiometricID,
    bool AttControlled,
    bool AccControlled,
    bool JobControlled,
    bool ExtControlled,
    bool RiskControlled,
    string? WebLogin,
    string? WebPassword,
    bool ActiveDirectory,
    List<UserFieldDto>? UserFields
);

public record UserFieldDto(string FieldDefinitionId, string Value);
