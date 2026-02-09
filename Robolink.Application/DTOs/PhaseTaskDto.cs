using Robolink.Core.Enums;

namespace Robolink.Application.DTOs
{
    public class PhaseTaskDto
    {
        public string Name { get; set; } = null!;
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string? PhaseName { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public string AssignedStaffName { get; set; } = null!;
        public Task_Status Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        
        // ✅ NEW: Add progress tracking properties
        public int ProcessRate { get; set; } = 0;  // 0-100%
        public DateTime? CompletedAt { get; set; }
        public decimal EstimatedHours { get; set; } = 0;
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public int Priority { get; set; } = 0;  // ✅ NEW: 0=Low, 1=Medium, 2=High, 3=Critical
        
        
        // ✅ NEW: Add parent task support
        public Guid? ParentPhaseTaskId { get; set; }
        public string? ParentPhaseTaskName { get; set; }
        // ✅ NEW: Add RowVersion for concurrency control
        public List<PhaseTaskDto> SubPhaseTasks { get; set; } = new();
    }

    public class CreatePhaseTaskRequest
    {
        public string Name { get; set; } = null!;
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public int ProcessRate { get; set; } = 0;  // 0-100%
        public int Status { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public string AssignedStaffName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal EstimatedHours { get; set; } = 0;
        public int Priority { get; set; } = 0;
        public Guid? ParentPhaseTaskId { get; set; }
    }
}