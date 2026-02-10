using Robolink.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Robolink.Application.DTOs
{
    public class PhaseTaskDto
    {
        public string Name { get; set; } = null!;  // ✅ Non-nullable
        public Guid Id  { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string? PhaseName { get; set; }
        public string Description { get; set; } = null!;  // ✅ Non-nullable
        public Guid AssignedStaffId { get; set; }
        public string? AssignedStaffName { get; set; }
        public Task_Status Status { get; set; }  // ✅ Non-nullable
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        
        public int ProcessRate { get; set; } = 0;
        public DateTime? CompletedAt { get; set; }
        public decimal EstimatedHours { get; set; } = 0;
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public int Priority { get; set; } = 0;  // ✅ Non-nullable
        
        public Guid? ParentPhaseTaskId { get; set; }
        public string? ParentPhaseTaskName { get; set; }
        public List<PhaseTaskDto> SubPhaseTasks { get; set; } = new();
    }

    public class CreatePhaseTaskRequest
    {
        [Required(ErrorMessage = "Task Name is required")]
        [StringLength(200, ErrorMessage = "Task Name cannot exceed 200 characters")]
        public string Name { get; set; } = null!;

        public decimal? InternalBudget { get; set; }

        public decimal? CustomerBudget { get; set; }

        [Range(0, 100, ErrorMessage = "Process rate must be between 0 and 100")]
        public int ProcessRate { get; set; } = 0;

        [Required]
        public int Status { get; set; }

        [Required(ErrorMessage = "Project ID is required")]
        public Guid ProjectId { get; set; }

        [Required(ErrorMessage = "Phase Configuration is required")]
        public Guid ProjectSystemPhaseConfigId { get; set; }

        [StringLength(1000, ErrorMessage = "Description is too long")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Please assign a staff member")]
        public Guid AssignedStaffId { get; set; }

        // Thông tin này thường lấy từ StaffId, nhưng nếu cần truyền text thì giữ nguyên
        public string AssignedStaffName { get; set; } = null!;

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; }

        [Range(0, 9999, ErrorMessage = "Estimated hours must be a positive number")]
        public decimal EstimatedHours { get; set; } = 0;

        [Required]
        public int Priority { get; set; } = 0;

        public Guid? ParentPhaseTaskId { get; set; }
    }
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