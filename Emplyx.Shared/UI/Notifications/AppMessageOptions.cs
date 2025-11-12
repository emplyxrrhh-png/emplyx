namespace Emplyx.Shared.UI.Notifications;

public sealed record AppMessageOptions(
    string Title,
    string Message,
    string ConfirmText = "Aceptar",
    string? CancelText = null,
    UiNotificationLevel Level = UiNotificationLevel.Info);
