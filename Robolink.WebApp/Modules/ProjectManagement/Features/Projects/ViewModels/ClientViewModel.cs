namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

/// <summary>
/// Represents a client in the projects context.
/// </summary>
public class ClientViewModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ContactEmail { get; set; }
    public string? Industry { get; set; }
    // Thêm trường này nếu em muốn hiển thị số lượng dự án trên UI
    public int ProjectCount { get; set; }
}