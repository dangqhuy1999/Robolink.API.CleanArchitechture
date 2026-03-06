using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Modals;

/// <summary>
/// Encapsulates all state for the CreateProjectModal component.
/// Single Source of Truth for modal state.
/// </summary>
public class CreateProjectModalState
{
    // ===== FORM STATE =====
    /// <summary>
    /// Form data model.
    /// </summary>
    public CreateProjectViewModel FormData { get; set; } = new();

    // ===== UI STATE =====
    /// <summary>
    /// Indicates if data is being loaded from API.
    /// </summary>
    public bool IsLoadingData { get; set; } = false;

    /// <summary>
    /// Indicates if form is being submitted.
    /// </summary>
    public bool IsSubmitting { get; set; } = false;

    // ===== DROPDOWN DATA =====
    /// <summary>
    /// Available clients for selection.
    /// </summary>
    public List<ClientViewModel> AvailableClients { get; set; } = [];

    /// <summary>
    /// Available managers for selection.
    /// </summary>
    public List<StaffViewModel> AvailableManagers { get; set; } = [];

    // ===== VALIDATION STATE =====
    /// <summary>
    /// Validation errors by field name.
    /// </summary>
    public Dictionary<string, string> ValidationErrors { get; set; } = [];

    /// <summary>
    /// Last error message (for general form errors).
    /// </summary>
    public string? LastErrorMessage { get; set; }

    // ===== COMPUTED PROPERTIES =====

    /// <summary>
    /// Indicates if form is ready for submission.
    /// </summary>
    public bool IsFormValid => !HasValidationErrors && !IsSubmitting;

    /// <summary>
    /// Indicates if there are validation errors.
    /// </summary>
    public bool HasValidationErrors => ValidationErrors.Count > 0;

    /// <summary>
    /// Indicates if the modal is processing (loading or submitting).
    /// </summary>
    public bool IsProcessing => IsLoadingData || IsSubmitting;

    /// <summary>
    /// Gets the appropriate modal title based on project type.
    /// </summary>
    public string ModalTitle =>
        FormData.IsSubProject ? "Create Sub-Project" : "Create Project";

    /// <summary>
    /// Gets the appropriate button text based on state.
    /// </summary>
    public string SubmitButtonText =>
        IsSubmitting ? "Creating..." : FormData.IsSubProject ? "Create Sub-Project" : "Create Project";

    // ===== RESET METHODS =====

    /// <summary>
    /// Resets form to initial state.
    /// </summary>
    public void ResetForm()
    {
        FormData = new CreateProjectViewModel();
        ValidationErrors.Clear();
        LastErrorMessage = null;
        IsSubmitting = false;
    }

    /// <summary>
    /// Resets all state to initial values.
    /// </summary>
    public void ResetAll()
    {
        ResetForm();
        AvailableClients.Clear();
        AvailableManagers.Clear();
        IsLoadingData = false;
    }

    /// <summary>
    /// Clears validation errors for a specific field.
    /// </summary>
    public void ClearFieldError(string fieldName)
    {
        ValidationErrors.Remove(fieldName);
    }

    /// <summary>
    /// Sets validation error for a field.
    /// </summary>
    public void SetFieldError(string fieldName, string errorMessage)
    {
        ValidationErrors[fieldName] = errorMessage;
    }
}