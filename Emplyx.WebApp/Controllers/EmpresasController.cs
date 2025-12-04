using System;
using Emplyx.Application.Abstractions;
using Emplyx.Shared.Contracts.Empresas;
using Microsoft.AspNetCore.Mvc;

namespace Emplyx.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _empresaService;

    public EmpresasController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmpresaResponse>>> GetAll([FromQuery] Guid? tenantId, CancellationToken cancellationToken)
    {
        var empresas = tenantId.HasValue
            ? await _empresaService.GetByTenantIdAsync(tenantId.Value, cancellationToken)
            : await _empresaService.GetAllAsync(cancellationToken);

        return Ok(empresas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmpresaResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var empresa = await _empresaService.GetByIdAsync(id, cancellationToken);
        if (empresa is null)
        {
            return NotFound();
        }
        return Ok(empresa);
    }

    [HttpPost]
    public async Task<ActionResult<EmpresaResponse>> Create(CreateEmpresaRequest request, CancellationToken cancellationToken)
    {
        var empresa = await _empresaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmpresaRequest request, CancellationToken cancellationToken)
    {
        await _empresaService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _empresaService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
