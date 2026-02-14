using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.Projects
{
    public class GetProjectByIdQuery : IRequest<ProjectDto?>
    {
        public Guid ProjectId { get; set; }

        public GetProjectByIdQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}