namespace Emplyx.Shared.UI.Notifications;

public sealed record AppToastOptions(
    string Title,
    string Message,
    UiNotificationLevel Level = UiNotificationLevel.Info,
    int DurationMs = 4000);
