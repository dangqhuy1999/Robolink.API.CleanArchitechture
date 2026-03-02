using MediatR;

namespace Robolink.Application.Commands.ProjectPhases
{
    public record DeleteProjectPhaseCommand(Guid PhaseConfigId) : IRequest<bool>;
}