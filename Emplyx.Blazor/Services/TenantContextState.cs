using Emplyx.Shared.UI;

namespace Emplyx.Blazor.Services;

public sealed class TenantContextState : ITenantContextAccessor, IDisposable
{
    private TenantContext _current = TenantContext.Default;

    public TenantContext Current => _current;

    public event Action<TenantContext>? ContextChanged;

    public Task SetAsync(TenantContext context, CancellationToken cancellationToken = default)
    {
        _current = context;
        ContextChanged?.Invoke(_current);
        return Task.CompletedTask;
    }

    public void Dispose() => ContextChanged = null;
}
