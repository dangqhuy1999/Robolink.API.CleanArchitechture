using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Robolink.Shared.DTOs
{
    /// <summary>DTO for displaying project</summary>
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
        public int Status { get; set; }
        public int Priority { get; set; }
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public string? CalculationConfigJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        
        // ✅ NEW: Parent-Sub Project Support
        public Guid? ParentProjectId { get; set; }
        public string? ParentProjectName { get; set; }
        public List<ProjectDto> SubProjects { get; set; } = new();
    }

    /// <summary>DTO for creating a new project</summary>
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Project Code is required")]
        [StringLength(50)]
        public string ProjectCode { get; set; } = null!;

        [Required(ErrorMessage = "Project Name is required")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Client is required")]
        public Guid ClientId { get; set; }

        [Required(ErrorMessage = "Manager is required")]
        public Guid ManagerId { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Deadline is required")]
        public DateTime Deadline { get; set; }

        [Required]
        public int Priority { get; set; } = 1; // Default: Medium

        public int Status { get; set; } = 0; // Default: Draft

        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public string? CalculationConfigJson { get; set; }
        // ✅ NEW: Optional parent project
        public Guid? ParentProjectId { get; set; }
    }

    /// <summary>DTO for updating a project</summary>
    public class UpdateProjectRequest
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public Guid? ManagerId { get; set; }

        public DateTime? Deadline { get; set; }

        public int? Priority { get; set; }
        public int? Status { get; set; }

        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        // ✅ NEW: Allow changing parent project
        public Guid? ParentProjectId { get; set; }
    }

    /// <summary>Response from CRUD operations</summary>
    public class CrudResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}