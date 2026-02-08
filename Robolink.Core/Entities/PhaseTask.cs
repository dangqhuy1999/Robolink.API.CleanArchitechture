using Robolink.Core.Common;
using Robolink.Core.Enums;
using System;

namespace Robolink.Core.Entities
{
    public class PhaseTask : EntityRootBase
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Task_Status Status { get; set; } = Task_Status.Pending;
        public int Priority { get; set; } = 0; // ✅ NEW: 0=Low, 1=Medium, 2=High, 3=Critical    
        // ✅ NEW: Add these properties for progress tracking
        public int ProcessRate { get; set; } = 0;  // 0-100%
        public DateTime? CompletedAt { get; set; }
        public decimal EstimatedHours { get; set; } = 0;
        public decimal? InternalBudget { get; set; }
        public decimal? CustomerBudget { get; set; }
        public Guid? ParentPhaseTaskId { get; set; }
        // Foreign Keys & Navigation
        public virtual Project? Project { get; set; }
        public virtual ProjectSystemPhaseConfig? PhaseConfig { get; set; }
        public virtual Staff? AssignedStaff { get; set; }
        public virtual PhaseTask? ParentPhaseTask { get; set; }
        public virtual ICollection<PhaseTask> SubPhaseTasksItems { get; set; } = new List<PhaseTask>();
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
