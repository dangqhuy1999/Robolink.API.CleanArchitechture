using MediatR;
using Microsoft.Extensions.Logging;
using Robolink.Application.Events;

namespace Robolink.Application.EventHandlers
{
    /// <summary>Handle ProjectCreatedEvent (e.g., send notification, log audit)</summary>
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly ILogger<ProjectCreatedEventHandler> _logger;

        public ProjectCreatedEventHandler(ILogger<ProjectCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ProjectCreatedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🎉 Project Created: {@event.ProjectName} (ID: {@event.ProjectId})");
            // TODO: Send email, publish to message queue, etc.
            return Task.CompletedTask;
        }
    }
}