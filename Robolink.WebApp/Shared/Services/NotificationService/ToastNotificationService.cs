using Microsoft.JSInterop;

namespace Robolink.WebApp.Shared.Services.NotificationService;

/// <summary>
/// Provides toast notification functionality using JavaScript interop.
/// Integrates with a JavaScript toast library (e.g., Toastr, SweetAlert2, or Bootstrap Toast).
/// </summary>
public class ToastNotificationService : IToastNotificationService
{
    private readonly IJSRuntime _jsRuntime;
    private const string ShowToastMethodName = "showToast";
    private const int DefaultDuration = 3000;

    public ToastNotificationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <inheritdoc />
    public async Task ShowSuccessAsync(string message, int duration = 3000)
    {
        await ShowNotificationAsync("success", message, duration);
    }

    /// <inheritdoc />
    public async Task ShowErrorAsync(string message, int duration = 5000)
    {
        await ShowNotificationAsync("error", message, duration);
    }

    /// <inheritdoc />
    public async Task ShowWarningAsync(string message, int duration = 4000)
    {
        await ShowNotificationAsync("warning", message, duration);
    }

    /// <inheritdoc />
    public async Task ShowInfoAsync(string message, int duration = 3000)
    {
        await ShowNotificationAsync("info", message, duration);
    }
    /// <summary>
    /// Shows a toast notification with fallback error handling.
    /// </summary>
    private async Task ShowNotificationAsync(string type, string message, int duration)
    {
        try
        {
            // Call JavaScript function: window.showToast('success', 'message', 3000)
            await _jsRuntime.InvokeVoidAsync(ShowToastMethodName, type, message, duration);
        }
        catch (JSDisconnectedException)
        {
            // WebSocket disconnected - log but don't throw
            Console.WriteLine($"[Toast] JS Runtime disconnected - Message was: {message}");
        }
        catch (TaskCanceledException)
        {
            // JS call timed out - log but don't throw
            Console.WriteLine($"[Toast] JS call timed out - Message was: {message}");
        }
        catch (Exception ex)
        {
            // Fallback: log to console if toast fails
            await _jsRuntime.InvokeVoidAsync("console.error", 
                $"Toast notification failed: {type} - {message} - {ex.Message}");
        }
    }
}