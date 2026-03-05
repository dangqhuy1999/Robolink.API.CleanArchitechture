using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.WebApp.Shared.Services.ApiError
{
    /// <summary>
    /// Handles API exceptions with consistent error messaging.
    /// </summary>
    public interface IApiErrorHandler
    {
        /// <summary>
        /// Handles an API exception and shows appropriate error notification.
        /// </summary>
        /// <param name='ex'>The exception to handle.</param>
        /// <param name="contextMessage">Context message for error logging.</param>
        Task HandleAsync(Exception ex, string contextMessage = "");
    }
}
