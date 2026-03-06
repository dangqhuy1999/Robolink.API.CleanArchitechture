using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Pages;

/// <summary>
/// Encapsulates all state for the Projects page.
/// Single Source of Truth for component state.
/// </summary>
public class ProjectsPageState
{
    
    // ===== UI STATE =====
    public bool IsLoading { get; set; } = true;
    
    // ===== DATA STATE =====
    public List<ProjectViewModel> Projects { get; set; } = [];
    
    // ===== SELECTION STATE =====
    public Guid SelectedProjectId { get; set; } = Guid.Empty;
    
    // ===== PAGINATION STATE =====
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalProjects { get; set; } = 0;
    public int TotalPages { get; set; } = 0;
    
    // ===== MODAL STATE =====
    public ModalVisibility ModalState { get; set; } = new();

    // ===== COMPUTED PROPERTIES =====
    /*
    public bool IsFirstPage => CurrentPage == 1;
    public bool IsLastPage => CurrentPage >= TotalPages;
    public int SkipCount => (CurrentPage - 1) * PageSize;
    public string PageInfoText => $"Page {CurrentPage} of {TotalPages} | Total: {TotalProjects} projects";
    */

    /// <summary>
    /// Resets pagination to first page.
    /// </summary>
    public void ResetPagination()
    {
        CurrentPage = 1;
        TotalPages = 0;
        TotalProjects = 0;
    }

    /// <summary>
    /// Resets all state to initial values.
    /// </summary>
    public void ResetAll() 
    {
        Projects.Clear();
        SelectedProjectId = Guid.Empty;
        ResetPagination();
        ModalState.ResetAll();
    }
}

/// <summary>
/// Encapsulates modal visibility state.
/// </summary>
public class ModalVisibility
{
    public bool ShowCreateModal { get; set; }
    public bool ShowEditModal { get; set; }
    public bool ShowQuickAddSubModal { get; set; }

    public void ResetAll()
    {
        ShowCreateModal = false;
        ShowEditModal = false;
        ShowQuickAddSubModal = false;
    }
}