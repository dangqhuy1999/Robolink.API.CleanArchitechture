public class PhaseTask : EntityBase
{
    public Guid ProjectId { get; set; }
    public Guid ProjectSystemPhaseConfigId { get; set; }
    public string Description { get; set; }
    public Guid AssignedStaffId { get; set; }
    public DateTime DueDate { get; set; }
    public Task_Status Status { get; set; }
    public int ProcessRate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal EstimatedHours { get; set; }
    public Guid? ParentPhaseTaskId { get; set; } // ? Add this property to fix CS1061
    public virtual Project? Project { get; set; }
    public virtual ProjectSystemPhaseConfig? PhaseConfig { get; set; }
    public virtual Staff? AssignedStaff { get; set; }
    public virtual PhaseTask? ParentPhaseTask { get; set; }
    public virtual ICollection<PhaseTask> SubPhaseTasksItems { get; set; }
    public virtual ICollection<WorkLog> WorkLogs { get; set; }
}