using System.Security.Claims;

namespace Emplyx.Blazor.Services.Security;

public interface IPermissionService
{
    bool Has(string permission);
}

public sealed class PermissionService : IPermissionService
{
    private readonly ClaimsPrincipal _user;
    private readonly HashSet<string> _granted = new(StringComparer.OrdinalIgnoreCase);

    public PermissionService(ClaimsPrincipal user)
    {
        _user = user;
        // Mock: populate with sample permissions (would come from claims / API)
        _granted.Add("employees.view");
        _granted.Add("employees.export");
        _granted.Add("access.groups.view");
        _granted.Add("scheduler.coverage.view");
    }

    public bool Has(string permission) => _granted.Contains(permission);
}
