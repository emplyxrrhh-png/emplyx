using Emplyx.Shared.UI.Notifications;

namespace Emplyx.Blazor.Services;

public sealed class AppUiMessagingService
{
    public sealed class AppMessageTicket
    {
        private readonly TaskCompletionSource<bool?> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal AppMessageTicket(AppMessageOptions options) => Options = options;

        public AppMessageOptions Options { get; }

        public Task<bool?> WaitAsync() => _completion.Task;

        internal void Complete(bool? result) => _completion.TrySetResult(result);
    }

    public event Action<AppMessageTicket>? MessageRequested;
    public event Action<AppToastOptions>? ToastRequested;

    public Task<bool?> ShowMessageAsync(AppMessageOptions options)
    {
        var ticket = new AppMessageTicket(options);
        var handler = MessageRequested;
        if (handler is null)
        {
            ticket.Complete(false);
            return ticket.WaitAsync();
        }

        handler.Invoke(ticket);
        return ticket.WaitAsync();
    }

    public Task<bool?> ShowConfirmationAsync(string title, string message, string confirmText = "Aceptar", string cancelText = "Cancelar") =>
        ShowMessageAsync(new AppMessageOptions(title, message, confirmText, cancelText, UiNotificationLevel.Warning));

    public void ShowToast(AppToastOptions options)
    {
        ToastRequested?.Invoke(options);
    }
}
