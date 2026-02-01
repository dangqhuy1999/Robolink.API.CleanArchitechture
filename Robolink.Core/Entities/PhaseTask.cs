using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Robolink.Core.Enums;

namespace Robolink.Core.Entities
{
    public class PhaseTask : EntityBase
    {
        public Guid ProjectId { get; set; } // Khóa ngoại tới Project
        [ForeignKey("ProjectId")] // Chỉ định rõ cho EF Core
        public virtual Project? Project { get; set; }

        // ✅ NEW: Parent subtask (optional - for hierarchical subtasks)
        public Guid? ParentPhaseTaskId { get; set; }
        [ForeignKey("ParentPhaseTaskId")]
        public virtual PhaseTask? ParentPhaseTask { get; set; }

        // Child subtasks
        public virtual ICollection<PhaseTask> SubPhaseTasksItems { get; set; } = new List<PhaseTask>();

        // ✅ NEW: Reference to project-specific phase config (not just PhaseId)
        public Guid ProjectSystemPhaseConfigId { get; set; }
        [ForeignKey("ProjectSystemPhaseConfigId")]
        public virtual ProjectSystemPhaseConfig? PhaseConfig { get; set; }

        public string? Description { get; set; }
        public Guid AssignedStaffId { get; set; }
        // [THÊM MỚI] Để EF Core biết AssignedStaffId trỏ sang bảng Staff
        [ForeignKey("AssignedStaffId")] // Chỉ định rõ cho EF Core
        public virtual Staff? AssignedStaff { get; set; }
        public int ProcessRate { get; set; } // 0-100
        public DateTime DueDate { get; set; }
        public Task_Status Status { get; set; }

        // Để từ 1 Task, em xem được ngay danh sách tất cả các lần gáp ốc của Robot
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
    
}
