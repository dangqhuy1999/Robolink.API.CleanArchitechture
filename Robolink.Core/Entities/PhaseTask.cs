using Robolink.Core.Common;
using Robolink.Shared.Enums;
using System;

namespace Robolink.Core.Entities
{
    public class PhaseTask : EntityRootBase
    {
        // 1. Constructor không tham số (BẮT BUỘC cho EF Core)
        // EF Core dùng cái này để "đổ" dữ liệu từ DB vào Object.
        // Chúng ta để protected hoặc public đều được.
        public PhaseTask()
        {
            SubPhaseTasksItems = new List<PhaseTask>();
            WorkLogs = new List<WorkLog>();
        }

        // 2. Constructor dùng để tạo mới (Custom Constructor)
        // Khi em dùng lệnh: var task = new PhaseTask("Tên task", ...);
        public PhaseTask(string name, Guid projectId, Guid phaseId) : this()
        {
            Id = Guid.NewGuid(); // Tự sinh ID ở đây
            Name = name;
            ProjectId = projectId;
            ProjectSystemPhaseConfigId = phaseId;
            Status = Task_Status.Pending; // Giá trị mặc định
            StartDate = DateTime.Now;
        }
        public string Name { get; set; } = null!;
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
