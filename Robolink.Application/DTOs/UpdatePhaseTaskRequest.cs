using Robolink.Core.Enums;

namespace Robolink.Application.DTOs
{
    public class UpdatePhaseTaskRequest
    {
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }            // match PhaseTaskDto.Status
        public int ProcessRate { get; set; } = 0; // 0-100%
        public int Priority { get; set; } = 0;    // ✅ NEW: 0=Low, 1=Medium, 2=High, 3=Critical
        public decimal EstimatedHours { get; set; } = 0;
        public Guid? ParentPhaseTaskId { get; set; }  // ✅ NEW: For sub-tasks
        public byte[]? RowVersion { get; set; }   // optional concurrency token
    }
}