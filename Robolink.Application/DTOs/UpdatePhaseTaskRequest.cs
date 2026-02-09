using Robolink.Core.Enums;

namespace Robolink.Application.DTOs
{
    public class UpdatePhaseTaskRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public DateTime DueDate { get; set; }
        public Task_Status Status { get; set; }            // match PhaseTaskDto.Status
        public int Priority { get; set; } = 0;    // ✅ NEW: 0=Low, 1=Medium, 2=High, 3=Critical
        public decimal EstimatedHours { get; set; } = 0;
        public Guid? ParentPhaseTaskId { get; set; }  // ✅ NEW: For sub-tasks
    }
}