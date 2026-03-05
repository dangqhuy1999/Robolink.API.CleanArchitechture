using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

/// <summary>
/// Represents a project in the UI layer.
/// Decoupled from API DTO to allow independent UI evolution.
/// </summary>
public class ProjectViewModel
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Project name/title.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Detailed description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Client ID associated with this project.
    /// </summary>
    public Guid ClientId { get; set; }

    /// <summary>
    /// Display name of the client.
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// Project manager ID.
    /// </summary>
    public Guid ManagerId { get; set; }

    /// <summary>
    /// Display name of the project manager.
    /// </summary>
    public string ManagerName { get; set; } = string.Empty;

    /// <summary>
    /// Project start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project deadline.
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// Priority level (1-5).
    /// </summary>
    public ProjectPriority Priority { get; set; }

    /// <summary>
    /// Current status of the project.
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Parent project ID (if this is a sub-project).
    /// </summary>
    public Guid? ParentProjectId { get; set; }

    /// <summary>
    /// Number of sub-projects.
    /// </summary>
    public int SubProjectsCount { get; set; }

    /// <summary>
    /// Collection of sub-projects.
    /// </summary>
    public List<ProjectViewModel> SubProjects { get; set; } = [];

    // ===== UI-Specific Properties (NOT from DTO) =====

    /// <summary>
    /// Indicates if this is a parent project (has no parent).
    /// </summary>
    public bool IsParentProject => ParentProjectId == null || ParentProjectId == Guid.Empty;

    public bool HasSubProjects => SubProjectsCount > 0;
    /// <summary>
    /// Calculates remaining days until deadline.
    /// </summary>
    public int DaysUntilDeadline => (Deadline - DateTime.UtcNow).Days;

    /// <summary>
    /// Indicates if project is overdue.
    /// </summary>
    public bool IsOverdue => DateTime.UtcNow > Deadline;

    /// <summary>
    /// Indicates if project is in critical status (overdue or 3 days left).
    /// </summary>
    public bool IsCritical => IsOverdue || DaysUntilDeadline <= 3;

    /// <summary>
    /// Gets CSS class for status badge.
    /// </summary>
    public string StatusBadgeClass => Status switch
    {
        ProjectStatus.InProgress => "bg-success",
        ProjectStatus.Draft => "bg-warning",
        ProjectStatus.OnHold => "bg-secondary",
        ProjectStatus.Completed => "bg-info",
        _ => "bg-light text-dark"
    };

    public string? ProjectCode { get; set; } // Thêm mã dự án

    // UI Helper
    public string PriorityText => Priority switch
    {
        ProjectPriority.Low => "Low",
        ProjectPriority.Medium => "Medium",
        ProjectPriority.High => "High",
        ProjectPriority.Ugent => "Urgent",
        _ => "Critical"
    };

    public string PriorityBadgeClass => Priority switch
    {
        >= ProjectPriority.Ugent => "badge-danger",
        ProjectPriority.High => "badge-warning",
        _ => "badge-info"
    };

    public decimal? InternalBudget { get; set; }
    public decimal? CustomerBudget { get; set; }
    // Trong ProjectViewModel
    public double ProgressPercentage { get; set; }

    // Chỉ cần 1 thuộc tính để hiển thị text nếu cần
    public string ProgressDisplayText => $"{Math.Round(ProgressPercentage, 1)}%";
    // Thêm cái này để hiển thị màu sắc thanh Progress Bar trên UI
    public string ProgressColorClass => ProgressPercentage switch
    {
        < 30 => "bg-danger",   // Đỏ: Chậm trễ/Mới bắt đầu
        < 70 => "bg-warning",  // Vàng: Đang làm
        _ => "bg-success"      // Xanh: Sắp xong
    };

}