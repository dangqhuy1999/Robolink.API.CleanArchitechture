namespace Robolink.Shared.DTOs
{
    public class SystemPhaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DefaultSequence { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }

    public class ProjectPhaseConfigDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid SystemPhaseId { get; set; }
        public SystemPhaseDto SystemPhase { get; set; } = null!;
        public string? CustomPhaseName { get; set; }
        public int Sequence { get; set; }
        public bool IsEnabled { get; set; }
        public int TaskCount { get; set; }
        public List<PhaseTaskDto> Tasks { get; set; } = new();
    }
}