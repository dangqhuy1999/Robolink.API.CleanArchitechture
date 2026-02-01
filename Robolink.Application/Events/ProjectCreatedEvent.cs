using MediatR;

namespace Robolink.Application.Events
{
    /// <summary>Domain event fired when project is created</summary>
    public class ProjectCreatedEvent : INotification
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}