using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.ProjectPhases
{
    public record UpdateProjectPhaseConfigCommand(UpdateProjectPhaseConfigRequest Request)
    : IRequest<ProjectPhaseConfigDto>;
}