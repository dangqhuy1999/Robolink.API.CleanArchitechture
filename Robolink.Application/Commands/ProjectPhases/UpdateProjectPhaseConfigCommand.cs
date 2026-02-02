using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Commands.ProjectPhases
{
    public record UpdateProjectPhaseConfigCommand(
        Guid PhaseConfigId,
        string? CustomPhaseName,
        int Sequence,
        bool IsEnabled
    ) : IRequest<ProjectPhaseConfigDto>;
}