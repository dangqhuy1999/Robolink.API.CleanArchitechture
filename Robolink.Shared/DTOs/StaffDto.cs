using System;
using Robolink.Shared.Enums;

namespace Robolink.Shared.DTOs
{
    /// <summary>DTO for displaying Staff</summary>
    public class StaffDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!; // Đồng bộ tên
        public string Username { get; set; } = null!;
        // Ẩn PasswordHash khỏi các truy vấn thông thường nếu có thể
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ProjectDepartment Department { get; set; }
        public ProjectRole Role { get; set; }
        public StaffStatus Status { get; set; }
    }

    
}