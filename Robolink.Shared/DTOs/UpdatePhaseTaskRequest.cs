using Robolink.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Robolink.Shared.DTOs
{
    public class UpdatePhaseTaskRequest
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }

        public decimal? InternalBudget { get; set; }

        public decimal? CustomerBudget { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public Guid? AssignedStaffId { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public Task_Status? Status { get; set; }  // ✅ Keep nullable for optional updates

        public int? Priority { get; set; }

        [Range(0, 9999)]
        public decimal? EstimatedHours { get; set; }

        public Guid? ParentPhaseTaskId { get; set; }
    }
}
