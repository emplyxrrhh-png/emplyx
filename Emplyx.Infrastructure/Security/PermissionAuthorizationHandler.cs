using System.Security.Claims;
using Emplyx.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Emplyx.Infrastructure.Security;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsuarioRepository _usuarioRepository;

    public PermissionAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        IUsuarioRepository usuarioRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _usuarioRepository = usuarioRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        Guid? contextoId = null;

        if (httpContext != null && httpContext.Request.Headers.TryGetValue("X-Context-Id", out var contextIdValues))
        {
            if (Guid.TryParse(contextIdValues.FirstOrDefault(), out var parsedContextId))
            {
                contextoId = parsedContextId;
            }
        }

        var hasPermission = await _usuarioRepository.HasPermissionAsync(userId, requirement.Permission, contextoId);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
