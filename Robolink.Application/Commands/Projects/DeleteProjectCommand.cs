using MediatR;

namespace Robolink.Application.Commands.Projects
{
    public class DeleteProjectCommand : IRequest<bool>
    {
        public Guid ProjectId { get; set; }
        public bool HardDelete { get; set; } = false; // false = soft delete

        public DeleteProjectCommand(Guid projectId, bool hardDelete = false)
        {
            ProjectId = projectId;
            HardDelete = hardDelete;
        }
    }
}