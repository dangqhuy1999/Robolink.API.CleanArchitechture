using Microsoft.AspNetCore.Components;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Modals;

/// <summary>
/// Handles all form validation for CreateProjectModal.
/// Validates form data before submission.
/// </summary>
public partial class CreateProjectModal : ComponentBase
{
    /// <summary>
    /// Validates the entire form before submission.
    /// Returns true if valid, false otherwise.
    /// </summary>
    private bool ValidateForm()
    {
        State.ValidationErrors.Clear();
        State.LastErrorMessage = null;

        // Validate project name
        if (string.IsNullOrWhiteSpace(State.FormData.Name))
        {
            State.SetFieldError(nameof(State.FormData.Name), "Project name is required.");
        }
        else if (State.FormData.Name.Length > 200)
        {
            State.SetFieldError(nameof(State.FormData.Name), "Project name cannot exceed 200 characters.");
        }

        // Validate client selection
        if (State.FormData.ClientId == Guid.Empty)
        {
            State.SetFieldError(nameof(State.FormData.ClientId), "Please select a client.");
        }

        // Validate manager selection
        if (State.FormData.ManagerId == Guid.Empty)
        {
            State.SetFieldError(nameof(State.FormData.ManagerId), "Please select a project manager.");
        }

        // Validate dates
        if (State.FormData.StartDate >= State.FormData.Deadline)
        {
            State.SetFieldError(nameof(State.FormData.Deadline), "Deadline must be after start date.");
        }

        // Validate description length
        if (!string.IsNullOrEmpty(State.FormData.Description) && 
            State.FormData.Description.Length > 1000)
        {
            State.SetFieldError(nameof(State.FormData.Description), 
                "Description cannot exceed 1000 characters.");
        }

        // Validate priority
        if (((int)State.FormData.Priority)  < 1 || ((int)State.FormData.Priority) > 5)
        {
            State.SetFieldError(nameof(State.FormData.Priority), "Priority must be between 1 and 5.");
        }

        Logger.LogInformation("Form validation result. Valid: {IsValid}, Errors: {ErrorCount}",
            !State.HasValidationErrors, State.ValidationErrors.Count);

        return !State.HasValidationErrors;
    }

    /// <summary>
    /// Validates a single field when user changes it.
    /// Provides immediate feedback.
    /// </summary>
    private void ValidateField(string fieldName, object? fieldValue)
    {
        State.ClearFieldError(fieldName);

        switch (fieldName)
        {
            case nameof(State.FormData.Name):
                if (string.IsNullOrWhiteSpace(fieldValue?.ToString()))
                    State.SetFieldError(fieldName, "Project name is required.");
                break;

            case nameof(State.FormData.ClientId):
                if (fieldValue is Guid guid && guid == Guid.Empty)
                    State.SetFieldError(fieldName, "Please select a client.");
                break;

            case nameof(State.FormData.ManagerId):
                if (fieldValue is Guid guid && guid == Guid.Empty)
                    State.SetFieldError(fieldName, "Please select a manager.");
                break;

            case nameof(State.FormData.Deadline):
                if (State.FormData.StartDate >= State.FormData.Deadline)
                    State.SetFieldError(fieldName, "Deadline must be after start date.");
                break;
        }
    }

    /// <summary>
    /// Gets error message for a specific field.
    /// </summary>
    private string? GetFieldError(string fieldName)
    {
        return State.ValidationErrors.TryGetValue(fieldName, out var error) ? error : null;
    }

    /// <summary>
    /// Indicates if a field has validation error.
    /// </summary>
    private bool HasFieldError(string fieldName) => State.ValidationErrors.ContainsKey(fieldName);
}