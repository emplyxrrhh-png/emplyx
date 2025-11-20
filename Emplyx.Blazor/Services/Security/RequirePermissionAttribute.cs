using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Emplyx.Blazor.Services.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    public string Permission { get; }
    public RequirePermissionAttribute(string permission) => Permission = permission;
}

public sealed class SecureRouteView : RouteView
{
    [Inject] private IPermissionService PermissionService { get; set; } = default!;

    protected override void Render(RenderTreeBuilder builder)
    {
        var permissionAttributes = RouteData.PageType.GetCustomAttributes(typeof(RequirePermissionAttribute), true)
            .OfType<RequirePermissionAttribute>()
            .ToList();
        if (permissionAttributes.Count == 0 || permissionAttributes.Any(a => PermissionService.Has(a.Permission)))
        {
            base.Render(builder);
            return;
        }
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "alert alert-danger");
        builder.AddContent(2, "No tiene permisos para ver esta vista.");
        builder.CloseElement();
    }
}
