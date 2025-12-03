using Emplyx.Application.Abstractions;
using Emplyx.Shared.Contracts.Tenants;
using Microsoft.AspNetCore.Mvc;

namespace Emplyx.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TenantResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tenants = await _tenantService.GetAllAsync(cancellationToken);
        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetByIdAsync(id, cancellationToken);
        if (tenant is null)
        {
            return NotFound();
        }
        return Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<TenantResponse>> Create(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        await _tenantService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _tenantService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
