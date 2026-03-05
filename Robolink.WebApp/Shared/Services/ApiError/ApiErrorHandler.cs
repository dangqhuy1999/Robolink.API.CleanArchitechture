using Refit;
using Robolink.WebApp.Shared.Services.NotificationService;
using System.Net;

namespace Robolink.WebApp.Shared.Services.ApiError;

/// <summary>
/// Implements consistent API exception handling and error messaging.
/// Parses HTTP status codes and provides context-aware error notifications.
/// </summary>
public class ApiErrorHandler : IApiErrorHandler
{
    private readonly IToastNotificationService _notificationService;
    private readonly ILogger<ApiErrorHandler> _logger;

    public ApiErrorHandler(
        IToastNotificationService notificationService,
        ILogger<ApiErrorHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles any exception by logging and displaying appropriate user notification.
    /// </summary>
    public async Task HandleAsync(Exception ex, string contextMessage = "")
    {
        if (ex == null)
        {
            _logger.LogWarning("HandleAsync called with null exception");
            return;
        }

        string errorMessage = GetErrorMessage(ex);
        string logMessage = BuildLogMessage(ex, contextMessage);

        _logger.LogError(ex, logMessage);

        await _notificationService.ShowErrorAsync(errorMessage);
    }

    /// <summary>
    /// Extracts the most appropriate error message based on exception type and HTTP status.
    /// </summary>
    private static string GetErrorMessage(Exception ex) => ex switch
    {
        ApiException apiEx => GetApiErrorMessage(apiEx),
        HttpRequestException httpEx => GetHttpErrorMessage(httpEx),
        TaskCanceledException => "The request took too long. Please try again.",
        InvalidOperationException invEx => $"Invalid operation: {invEx.Message}",
        ArgumentException argEx => $"Invalid argument: {argEx.Message}",
        _ => $"An unexpected error occurred: {ex.Message}"
    };

    /// <summary>
    /// Parses ApiException (Refit) status codes and returns user-friendly messages.
    /// </summary>
    private static string GetApiErrorMessage(ApiException apiEx)
    {
        return apiEx.StatusCode switch
        {
            HttpStatusCode.BadRequest => 
                "Invalid request data. Please check your input and try again.",
            
            HttpStatusCode.Unauthorized => 
                "Your session has expired. Please log in again.",
            
            HttpStatusCode.Forbidden => 
                "You don't have permission to perform this action.",
            
            HttpStatusCode.NotFound => 
                "The requested resource was not found.",
            
            HttpStatusCode.Conflict => 
                "This resource already exists or conflicts with existing data.",
            
            HttpStatusCode.UnprocessableEntity => 
                "The request contains invalid data that cannot be processed.",
            
            HttpStatusCode.TooManyRequests => 
                "Too many requests. Please wait a moment and try again.",
            
            HttpStatusCode.InternalServerError => 
                "Server error occurred. Our team has been notified. Please try again later.",
            
            HttpStatusCode.ServiceUnavailable => 
                "The service is temporarily unavailable. Please try again in a few moments.",
            
            HttpStatusCode.GatewayTimeout => 
                "The server took too long to respond. Please try again.",
            
            _ => $"Server error (HTTP {(int)apiEx.StatusCode}): {apiEx.Message}"
        };
    }

    /// <summary>
    /// Handles generic HTTP request exceptions (network errors, etc).
    /// </summary>
    private static string GetHttpErrorMessage(HttpRequestException httpEx)
    {
        return httpEx.InnerException switch
        {
            HttpRequestException => 
                "Network connection error. Please check your internet connection.",
            TimeoutException => 
                "Connection timeout. Please check your network and try again.",
            _ => $"Connection error: {httpEx.Message}"
        };
    }

    /// <summary>
    /// Builds a detailed log message including context and exception details.
    /// </summary>
    private static string BuildLogMessage(Exception ex, string contextMessage)
    {
        var message = new System.Text.StringBuilder();
        message.Append("API Error Occurred");

        if (!string.IsNullOrWhiteSpace(contextMessage))
        {
            message.Append($" - Context: {contextMessage}");
        }

        if (ex is ApiException apiEx)
        {
            message.Append($" - HTTP Status: {(int)apiEx.StatusCode} {apiEx.StatusCode}");
            message.Append($" - URI: {apiEx.RequestMessage?.RequestUri}");
        }

        message.Append($" - Exception Type: {ex.GetType().Name}");

        return message.ToString();
    }
}
