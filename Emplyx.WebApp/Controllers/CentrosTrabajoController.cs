using System;
using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.CentrosTrabajo;
using Microsoft.AspNetCore.Mvc;

namespace Emplyx.WebApp.Controllers;

[ApiController]
[Route("api/centros-trabajo")]
public class CentrosTrabajoController : ControllerBase
{
    private readonly ICentroTrabajoService _service;
    private readonly IEmpresaService _empresaService;

    public CentrosTrabajoController(ICentroTrabajoService service, IEmpresaService empresaService)
    {
        _service = service;
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? empresaId, [FromQuery] Guid? tenantId)
    {
        if (empresaId.HasValue)
        {
            var result = await _service.GetByEmpresaIdAsync(empresaId.Value);
            return Ok(result);
        }

        if (tenantId.HasValue)
        {
            var tenantResults = await _service.GetByTenantIdAsync(tenantId.Value);
            return Ok(tenantResults);
        }

        var all = await _service.GetAllAsync();
        return Ok(all);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCentroTrabajoRequest request)
    {
        var empresa = await _empresaService.GetByIdAsync(request.EmpresaId);
        if (empresa is null)
        {
            return BadRequest($"La empresa {request.EmpresaId} no existe");
        }

        var centro = new CentroTrabajo(
            Guid.NewGuid(),
            request.Nombre,
            request.EmpresaId,
            request.InternalId,
            new Address(request.Address.Street, request.Address.ZipCode, request.Address.City, request.Address.Province, request.Address.Country),
            new Contact(request.Contact.Name, request.Contact.Phone, request.Contact.Email),
            request.TimeZone,
            request.Language
        );

        var result = await _service.CreateAsync(centro);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCentroTrabajoRequest request)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.Update(
            request.Nombre,
            request.InternalId,
            new Address(request.Address.Street, request.Address.ZipCode, request.Address.City, request.Address.Province, request.Address.Country),
            new Contact(request.Contact.Name, request.Contact.Phone, request.Contact.Email),
            request.TimeZone,
            request.Language,
            request.IsActive
        );

        await _service.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

public record CreateCentroTrabajoRequest(
    string Nombre,
    Guid EmpresaId,
    string InternalId,
    AddressDto Address,
    ContactDto Contact,
    string TimeZone,
    string Language
);

public record UpdateCentroTrabajoRequest(
    string Nombre,
    string InternalId,
    AddressDto Address,
    ContactDto Contact,
    string TimeZone,
    string Language,
    bool IsActive
);

public record AddressDto(string Street, string ZipCode, string City, string Province, string Country);
public record ContactDto(string Name, string Phone, string Email);
