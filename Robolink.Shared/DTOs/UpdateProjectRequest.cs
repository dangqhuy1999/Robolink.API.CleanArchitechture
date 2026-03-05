using Robolink.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Robolink.Shared.DTOs
{
    /// <summary>DTO for updating a project</summary>
    public class UpdateProjectRequest
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }
        [StringLength(50)]
        public string ProjectCode { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }

        public Guid? ManagerId { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }

        public ProjectPriority? Priority { get; set; }
        public ProjectStatus? Status { get; set; }

        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        // ✅ NEW: Allow changing parent project
        public Guid? ParentProjectId { get; set; }
    }
}
