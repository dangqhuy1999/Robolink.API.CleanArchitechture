using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Robolink.Shared.Enums;

namespace Robolink.Core.Entities
{
    public class Staff : EntityRootBase
    {
        public required string FullName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }

        // Ẩn PasswordHash khỏi các truy vấn thông thường nếu có thể
        public string PasswordHash { get; set; } = null!;

        public ProjectDepartment Department { get; set; } = ProjectDepartment.Production;
        public ProjectRole Role { get; set; } = ProjectRole.Operator;
        public StaffStatus Status { get; set; } = StaffStatus.Active;

        // Các mối quan hệ giữ nguyên như em làm (Rất tốt)
        public virtual ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public virtual ICollection<PhaseTask> AssignedTasks { get; set; } = new List<PhaseTask>();
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
    
