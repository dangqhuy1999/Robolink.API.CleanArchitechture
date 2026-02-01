using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Robolink.Core.Enums;

namespace Robolink.Core.Entities
{
    public class Staff : EntityRootBase
    {
        public string? FullName { set; get; }
        public  string? Username { get; set; } // Thêm cái này để login
        public  string? PasswordHash { get; set; } // Thêm cái này để lưu mật khẩu an toàn
        public ProjectDepartment Department { get; set; } = ProjectDepartment.Production;
        public ProjectRole Role { get; set; } = ProjectRole.Operator;
        public StaffStatus Status { get; set; } = StaffStatus.Active;

        // --- CÁC LIÊN KẾT ĐỂ KHỚP VỚI ERD ---
        // 1 Staff (Manager) quản lý nhiều Project
        public virtual ICollection<Project> ManagedProjects { get; set; } = new List<Project>();

        // 1 Staff thực hiện nhiều PhaseTask
        public virtual ICollection<PhaseTask> AssignedTasks { get; set; } = new List<PhaseTask>();

        // 1 Staff (Operator) tạo ra nhiều WorkLog
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
    

    
    
}
