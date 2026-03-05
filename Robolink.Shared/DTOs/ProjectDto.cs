
using Robolink.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Robolink.Shared.DTOs
{
    // <summary>DTO for displaying project</summary>
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string ProjectCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public ProjectStatus Status { get; set; }
        public ProjectPriority Priority { get; set; }
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public string? CalculationConfigJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        
        // ✅ NEW: Parent-Sub Project Support
        public Guid? ParentProjectId { get; set; }
        public string? ParentProjectName { get; set; }
        public List<ProjectDto> SubProjects { get; set; } = new();
        public int SubProjectsCount => SubProjects?.Count ?? 0;
        public double ProgressPercentage { get; set; } // Dùng double để lấy số thập phân nếu cần
    }

    

    

    /// <summary>Response from CRUD operations</summary>
    public class CrudResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}