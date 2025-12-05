using Emplyx.Application.Abstractions;
using Emplyx.Shared.Contracts.Usuarios;
using Microsoft.AspNetCore.Mvc;

namespace Emplyx.WebApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsersController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchUsuariosRequest request)
    {
        var result = await _usuarioService.SearchAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _usuarioService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioRequest request)
    {
        var result = await _usuarioService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _usuarioService.LoginAsync(request);
        if (result == null) return Unauthorized("Credenciales inválidas");
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUsuarioRequest request)
    {
        if (id != request.UsuarioId) return BadRequest("ID mismatch");
        
        var result = await _usuarioService.UpdateAsync(request);
        return Ok(result);
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] List<UsuarioRolAssignmentDto> roles)
    {
        // Since we don't have a specific method for just adding roles in the service, 
        // we fetch the user, add the roles to the existing ones, and call Update.
        // Ideally, we should have a specific method in the service.
        
        var user = await _usuarioService.GetByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = user.Roles.Select(r => new UsuarioRolAssignmentDto(r.RolId, r.ContextoId)).ToList();
        currentRoles.AddRange(roles);

        // We need to reconstruct the UpdateRequest. This is a bit clumsy but works without changing the service interface too much right now.
        // A better approach would be to add AssignRoleAsync to IUsuarioService.
        
        var updateRequest = new UpdateUsuarioRequest(
            user.Id,
            user.DisplayName,
            user.Email,
            user.IsActive,
            user.ClearanceId,
            null, // Password hash not updated here
            user.ExternalIdentityId,
            user.PreferredContextoId,
            user.Perfil,
            currentRoles,
            user.Contextos.Select(c => new UsuarioContextoAssignmentDto(c.ContextoId, c.IsPrimary)).ToList(),
            user.Licencias.Select(l => l.LicenciaId).ToList()
        );

        var result = await _usuarioService.UpdateAsync(updateRequest);
        return Ok(result);
    }
}
