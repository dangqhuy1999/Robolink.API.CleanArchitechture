using System;
using Robolink.Core.Enums;

namespace Robolink.Application.DTOs
{
    /// <summary>DTO for displaying Staff</summary>
    public class StaffDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public ProjectDepartment Department { get; set; }
        public ProjectRole Role { get; set; }
        public StaffStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}