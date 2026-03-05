using Microsoft.AspNetCore.Components;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Mappers;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;
using Robolink.WebApp.Shared.Services.NotificationService;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Modals;

/// <summary>
/// Handles all user interactions and events for CreateProjectModal.
/// Each method represents a single user action.
/// </summary>
public partial class CreateProjectModal : ComponentBase
{
    // ===== INJECTED SERVICES =====
    [Inject] private IProjectApi ProjectApi { get; set; } = null!;
    [Inject] private IToastNotificationService ToastNotificationService { get; set; } = null!;

    /// <summary>
    /// Handles form submission.
    /// Validates and creates the project.
    /// </summary>
    private async Task HandleSubmitAsync()
    {
        // Validate form
        if (!ValidateForm())
        {
            await ToastNotificationService.ShowWarningAsync("Please fix validation errors.");
            return;
        }

        try
        {
            State.IsSubmitting = true;
            Logger.LogInformation("Submitting project creation form");

            // Convert ViewModel to API Request
            var request = ProjectUiMapper.ToCreateRequest(State.FormData);

            // Call API
            var result = await ProjectApi.CreateAsync(request);

            Logger.LogInformation("Project created successfully. ID: {ProjectId}, Name: {ProjectName}",
                result.Id, result.Name);

            // Show success notification
            await ToastNotificationService.ShowSuccessAsync(
                $"Project '{result.Name}' created successfully!");

            // Close modal
            await HandleCloseAsync();

            // Notify parent component
            await OnSaved.InvokeAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating project");
            State.LastErrorMessage = "Failed to create project. Please try again.";
            await ToastNotificationService.ShowErrorAsync(State.LastErrorMessage);
        }
        finally
        {
            State.IsSubmitting = false;
        }
    }

    /// <summary>
    /// Handles closing the modal.
    /// Cleans up state and notifies parent.
    /// </summary>
    private async Task HandleCloseAsync()
    {
        Logger.LogInformation("Closing CreateProjectModal");
        State.ResetAll();
        await OnClose.InvokeAsync();
    }

    /// <summary>
    /// Handles form field changes.
    /// Updates state and validates field.
    /// </summary>
    private void HandleFieldChanged(string fieldName, object? value)
    {
        // Update form data based on field name
        switch (fieldName)
        {
            case nameof(State.FormData.Name):
                State.FormData.Name = value?.ToString() ?? string.Empty;
                break;
            case nameof(State.FormData.Description):
                State.FormData.Description = value?.ToString();
                break;
            case nameof(State.FormData.ClientId):
                if (Guid.TryParse(value?.ToString(), out var clientId))
                    State.FormData.ClientId = clientId;
                break;
            case nameof(State.FormData.ManagerId):
                if (Guid.TryParse(value?.ToString(), out var managerId))
                    State.FormData.ManagerId = managerId;
                break;
            case nameof(State.FormData.StartDate):
                if (DateTime.TryParse(value?.ToString(), out var startDate))
                    State.FormData.StartDate = startDate;
                break;
            case nameof(State.FormData.Deadline):
                if (DateTime.TryParse(value?.ToString(), out var deadline))
                    State.FormData.Deadline = deadline;
                break;
            case nameof(State.FormData.Priority):
                if (int.TryParse(value?.ToString(), out var priority))
                    State.FormData.Priority = priority  ;
                break;
        }

        // Validate the field
        ValidateField(fieldName, value);
        
        Logger.LogDebug("Field changed: {FieldName}", fieldName);
    }

    /// <summary>
    /// Handles client selection change.
    /// </summary>
    private void HandleClientSelected(ChangeEventArgs e)
    {
        HandleFieldChanged(nameof(State.FormData.ClientId), e.Value);
    }

    /// <summary>
    /// Handles manager selection change.
    /// </summary>
    private void HandleManagerSelected(ChangeEventArgs e)
    {
        HandleFieldChanged(nameof(State.FormData.ManagerId), e.Value);
    }

    /// <summary>
    /// Handles start date change.
    /// </summary>
    private void HandleStartDateChanged(ChangeEventArgs e)
    {
        HandleFieldChanged(nameof(State.FormData.StartDate), e.Value);
    }

    /// <summary>
    /// Handles deadline change.
    /// </summary>
    private void HandleDeadlineChanged(ChangeEventArgs e)
    {
        HandleFieldChanged(nameof(State.FormData.Deadline), e.Value);
    }

    /// <summary>
    /// Handles priority change.
    /// </summary>
    private void HandlePriorityChanged(ChangeEventArgs e)
    {
        HandleFieldChanged(nameof(State.FormData.Priority), e.Value);
    }
}