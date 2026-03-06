using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

/// <summary>
/// ViewModel for editing an existing project.
/// Tracks changes and original values for change detection.
/// </summary>
public class EditProjectViewModel
{
    /// <summary>
    /// Project ID (read-only after initialization).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Project name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    /// <summary>
    /// Project description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Client ID.
    /// </summary>
    public Guid ClientId { get; set; }

    /// <summary>
    /// Project manager ID.
    /// </summary>
    public Guid ManagerId { get; set; }

    /// <summary>
    /// Project start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project deadline.
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// Priority level.
    /// </summary>
    /// 
    public ProjectPriority Priority { get; set; }
    public ProjectStatus Status { get; set; }
    // ===== Original Values () =====

    public decimal? InternalBudget { get; set; }
    public decimal? CustomerBudget { get; set; }

    // ===== UI State =====

    /// <summary>
    /// Indicates if form is being submitted.
    /// </summary>
    public bool IsSubmitting { get; set; }

    /// <summary>
    /// Indicates if project data is loaded.
    /// </summary>
    public bool IsLoaded { get; set; }

    /// <summary>
    /// Available clients for dropdown.
    /// </summary>
    public List<ClientViewModel> AvailableClients { get; set; } = [];

    /// <summary>
    /// Available managers for dropdown.
    /// </summary>
    public List<StaffViewModel> AvailableManagers { get; set; } = [];

    // ===== Computed Properties(for change tracking) =====

    // Dữ liệu ràng buộc trực tiếp với các ô Input (Edit Form)
    public ProjectDto Data { get; set; } = new();

    // Bản sao lưu để đối chiếu
    private ProjectDto _backupData = new();

    public void LoadData(ProjectDto dto)
    {
        // 1. Gán cho Data để hiển thị lên form
        Data = dto;

        // 2. Tạo bản backup (Deep Copy)
        // Cách Senior: Nếu không muốn dùng thư viện phức tạp, hãy gán đủ các trường sửa được
        _backupData = new ProjectDto
        {
            Name = dto.Name,
            Description = dto.Description,
            ClientId = dto.ClientId,
            ManagerId = dto.ManagerId,
            StartDate = dto.StartDate,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            Status = dto.Status,
            InternalBudget = dto.InternalBudget,
            CustomerBudget = dto.CustomerBudget
        };
    }

    // Logic kiểm tra thay đổi đầy đủ
    public bool HasChanges =>
        Data.Name != _backupData.Name ||
        Data.Description != _backupData.Description ||
        Data.ClientId != _backupData.ClientId ||
        Data.ManagerId != _backupData.ManagerId ||
        Data.StartDate != _backupData.StartDate ||
        Data.Deadline != _backupData.Deadline ||
        Data.Priority != _backupData.Priority ||
        Data.Status != _backupData.Status ||
        Data.InternalBudget != _backupData.InternalBudget ||
        Data.CustomerBudget != _backupData.CustomerBudget;

    /*Sau này nếu dự án có thêm trường "Địa điểm" (Location), 
     * em chỉ cần cập nhật thêm vào LoadData và HasChanges của ViewModel này. 
     * Các file khác như ProjectDto hay ProjectEntity không bị ảnh hưởng.*/
}