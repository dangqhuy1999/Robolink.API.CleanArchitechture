namespace Robolink.Application.DTOs
{
    public class PhaseTaskDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string? PhaseName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public string AssignedStaffName { get; set; } = null!;
        public int ProcessRate { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePhaseTaskRequest
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectSystemPhaseConfigId { get; set; }
        public string Description { get; set; } = null!;
        public Guid AssignedStaffId { get; set; }
        public DateTime DueDate { get; set; }
    }
}