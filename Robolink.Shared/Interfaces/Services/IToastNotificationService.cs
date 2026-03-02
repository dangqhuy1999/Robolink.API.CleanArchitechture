using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.Interfaces.Services
{
    /// <summary>
    /// Provides toast notification functionality across the application.
    /// </summary>
    public interface IToastNotificationService
    {
        /// <summary>
        /// Shows a success toast notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds (default: 3000).</param>
        Task ShowSuccessAsync(string message, int duration = 3000);

        /// <summary>
        /// Shows an error toast notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds (default: 5000).</param>
        Task ShowErrorAsync(string message, int duration = 5000);

        /// <summary>
        /// Shows a warning toast notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds (default: 4000).</param>
        Task ShowWarningAsync(string message, int duration = 4000);

        /// <summary>
        /// Shows an info toast notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds (default: 3000).</param>
        Task ShowInfoAsync(string message, int duration = 3000);
    }
}
