using Robolink.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Robolink.Shared.DTOs
{
    public class PhaseTaskDto
    {
        public string Name { get; set; } = null!;  // ✅ Non-nullable
        public Guid Id  { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string Description { get; set; } = null!;  // ✅ Non-nullable
        public Guid AssignedStaffId { get; set; }
        public string AssignedStaffName { get; set; } = null!;  // ✅ Non-nullable
        public string PhaseName { get; set; } = null!;  // ✅ Non-nullable

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

    
    
}