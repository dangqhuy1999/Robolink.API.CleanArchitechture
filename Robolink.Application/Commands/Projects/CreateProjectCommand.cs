using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Commands.Projects
{
    public class CreateProjectCommand : IRequest<ProjectDto>
    {
        public CreateProjectRequest Request { get; set; } = null!;
        public string? CreatedBy     { get; set; }
    }
}