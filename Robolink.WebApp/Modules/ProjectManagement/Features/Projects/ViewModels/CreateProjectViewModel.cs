using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

/// <summary>
/// ViewModel for creating a new project.
/// Handles form state and validation.
/// </summary>
public class CreateProjectViewModel
{
    public string ProjectCode { get; set; } = null!;
    /// <summary>
    /// Project name (required).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Selected client ID.
    /// </summary>
    public Guid ClientId { get; set; }

    /// <summary>
    /// Selected project manager ID.
    /// </summary>
    public Guid ManagerId { get; set; }

    /// <summary>
    /// Project start date.
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Project deadline.
    /// </summary>
    public DateTime Deadline { get; set; } = DateTime.UtcNow.AddDays(30);

    /// <summary>
    /// Priority level (1-5).
    /// </summary>
    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    /// <summary>
    /// Parent project ID (optional, for sub-projects).
    /// </summary>
    public Guid? ParentProjectId { get; set; }

    public decimal? InternalBudget { get; set; }
    public decimal? CustomerBudget { get; set; }

    // ===== UI State =====

    /// <summary>
    /// Indicates if form is being submitted.
    /// </summary>
    public bool IsSubmitting { get; set; }

    /// <summary>
    /// Available clients for dropdown.
    /// </summary>
    public List<ClientViewModel> AvailableClients { get; set; } = [];

    /// <summary>
    /// Available managers for dropdown.
    /// </summary>
    public List<ManagerViewModel> AvailableManagers { get; set; } = [];

    // ===== Computed Properties =====

    /// <summary>
    /// Indicates if creating a sub-project.
    /// </summary>
    public bool IsSubProject => ParentProjectId.HasValue && ParentProjectId != Guid.Empty;

    /// <summary>
    /// Gets modal title based on project type.
    /// </summary>
    public string ModalTitle => IsSubProject ? "Create Sub-Project" : "Create Project";

    /// <summary>
    /// Indicates if form is valid.
    /// </summary>
    public bool IsFormValid =>
        !string.IsNullOrWhiteSpace(Name) &&
        ClientId != Guid.Empty &&
        ManagerId != Guid.Empty &&
        StartDate < Deadline;
}