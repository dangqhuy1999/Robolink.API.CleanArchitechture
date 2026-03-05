using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Pages;

/// <summary>
/// Handles all modal show/hide operations for the Projects page.
/// Each method pair manages one modal (Show/Hide).
/// </summary>
public partial class Projects : ComponentBase
{
    // ===== CREATE PROJECT MODAL =====

    private void HandleShowCreateModal()
    {
        State.ModalState.ShowCreateModal = true;
        Logger.LogInformation("Create project modal opened");
    }

    private void HandleHideCreateModal()
    {
        State.ModalState.ShowCreateModal = false;
        State.SelectedProjectId = Guid.Empty;
        Logger.LogInformation("Create project modal closed");
    }

    // ===== EDIT PROJECT MODAL =====

    private void HandleShowEditModal(Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            Logger.LogWarning("Attempted to edit project with invalid ID");
            return;
        }

        State.SelectedProjectId = projectId;
        State.ModalState.ShowEditModal = true;
        Logger.LogInformation("Edit project modal opened. ProjectId: {ProjectId}", projectId);
    }

    private void HandleHideEditModal()
    {
        State.ModalState.ShowEditModal = false;
        State.SelectedProjectId = Guid.Empty;
        Logger.LogInformation("Edit project modal closed");
    }

    // ===== QUICK ADD SUB-PROJECT MODAL =====

    private void HandleShowQuickAddSubModal(Guid parentProjectId)
    {
        if (parentProjectId == Guid.Empty)
        {
            Logger.LogWarning("Attempted to add sub-project with invalid parent ID");
            return;
        }

        State.SelectedProjectId = parentProjectId;
        State.ModalState.ShowQuickAddSubModal = true;
        Logger.LogInformation("Quick add sub-project modal opened. ParentId: {ParentId}", parentProjectId);
    }

    private void HandleHideQuickAddSubModal()
    {
        State.ModalState.ShowQuickAddSubModal = false;
        State.SelectedProjectId = Guid.Empty;
        Logger.LogInformation("Quick add sub-project modal closed");
    }

    // ===== MODAL CALLBACKS (Called from child components) =====

    /// <summary>
    /// Called when create modal is saved.
    /// Refreshes list and closes modal.
    /// </summary>
    private async Task HandleCreateModalSaved()
    {
        Logger.LogInformation("Project created via modal");
        HandleHideCreateModal();
        await HandleRefreshAsync();
    }

    /// <summary>
    /// Called when edit modal is saved.
    /// Refreshes list and closes modal.
    /// </summary>
    private async Task HandleEditModalSaved()
    {
        Logger.LogInformation("Project updated via modal");
        HandleHideEditModal();
        await HandleRefreshAsync();
    }

    /// <summary>
    /// Called when quick add sub-project modal is saved.
    /// Refreshes list and closes modal.
    /// </summary>
    private async Task HandleQuickAddSubModalSaved()
    {
        Logger.LogInformation("Sub-project created via modal");
        HandleHideQuickAddSubModal();
        await HandleRefreshAsync();
    }
}