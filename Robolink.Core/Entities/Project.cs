using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Robolink.Shared.Enums;

namespace Robolink.Core.Entities
{
    public class Project : EntityRootBase
    {
        public string? ProjectCode { get; set; } // "PRJ-2025-001"

        // ✅ NEW: Parent-sub project (optional - for hierarchical subprojects)
        public Guid? ParentProjectId { get; set; }
        [ForeignKey("ParentProjectId")]
        public virtual Project? ParentProject { get; set; }

        // Child subProjects
        public virtual ICollection<Project> SubProjectsItems { get; set; } = new List<Project>();

        public string Name { get; set; } = String.Empty;
        public Guid ClientId { get; set; }
        [ForeignKey("ClientId")] // Chỉ định rõ cho EF Core
        public virtual Client? Client { get; set; }
        public string Description { get; set; } = String.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }

        // Sử dụng Enum thay vì string để quản lý trạng thái chặt chẽ
        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

        public Guid ManagerId { get; set; }
        [ForeignKey("ManagerId")] // Chỉ định rõ cho EF Core
        // [THÊM MỚI] Để EF Core biết ManagerId trỏ sang bảng Staff
        public virtual Staff? Manager { get; set; }
        public decimal? InternalBudget { get; set; }
        public string? ContactPIC { get; set; }
        public decimal? CustomerBudget { get; set; }
        // Ánh xạ với CalculationConfigJson trong DB
        // Trong EF Core có thể dùng owned entity hoặc string chuyển đổi JSON
        public string? CalculationConfigJson { get; set; }
        public virtual ICollection<ProjectSystemPhaseConfig> PhaseConfigs { get; set; } = new List<ProjectSystemPhaseConfig>();

        public virtual ICollection<PhaseTask> Tasks { get; set; } = new List<PhaseTask>();
        // Thêm cái này để từ Project có thể lấy ngay tất cả các bản ghi sản xuất của Robot
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }

    
    
}
