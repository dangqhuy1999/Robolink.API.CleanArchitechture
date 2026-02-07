namespace Robolink.Application.DTOs
{
    public class PhaseTaskDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string? PhaseName { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public string AssignedStaffName { get; set; } = null!;
        public int Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // ✅ NEW: Add progress tracking properties
        public int ProcessRate { get; set; } = 0;  // 0-100%
        public DateTime? CompletedAt { get; set; }
        public decimal EstimatedHours { get; set; } = 0;
        public int Priority { get; set; } = 0;  // ✅ NEW: 0=Low, 1=Medium, 2=High, 3=Critical
        
        
        // ✅ NEW: Add parent task support
        public Guid? ParentPhaseTaskId { get; set; }
        
        // ✅ NEW: Add RowVersion for concurrency control
        public byte[]? RowVersion { get; set; }
    }

    public class CreatePhaseTaskRequest
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal EstimatedHours { get; set; } = 0;
        public int Priority { get; set; } = 0;
        public Guid? ParentPhaseTaskId { get; set; }
    }
}