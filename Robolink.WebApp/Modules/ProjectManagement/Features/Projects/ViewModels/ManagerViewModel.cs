using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

/// <summary>
/// Represents a project manager in the projects context.
/// </summary>
public class ManagerViewModel
{
    public Guid Id { get; set; }
    public required string FullName { get; set; } // Đổi từ Name -> FullName để khớp Dto
    public string? Email { get; set; }
    public ProjectDepartment Department { get; set; }
    public ProjectRole Role { get; set; }
}