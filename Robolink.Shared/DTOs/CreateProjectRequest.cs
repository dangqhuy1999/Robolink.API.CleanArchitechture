using Robolink.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Robolink.Shared.DTOs
{
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
        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium; // Default: Medium

        public ProjectStatus Status { get; set; } = ProjectStatus.Draft; // Default: Draft

        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public string? CalculationConfigJson { get; set; }
        // ✅ NEW: Optional parent project
        public Guid? ParentProjectId { get; set; }
    }
}
