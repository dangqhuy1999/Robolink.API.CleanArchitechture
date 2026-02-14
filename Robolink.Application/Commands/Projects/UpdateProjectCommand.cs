using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.Projects
{
    public class UpdateProjectCommand : IRequest<ProjectDto>
    {
        public UpdateProjectRequest Request { get; set; } = null!;
        public string? UpdatedBy { get; set; }
    }
}