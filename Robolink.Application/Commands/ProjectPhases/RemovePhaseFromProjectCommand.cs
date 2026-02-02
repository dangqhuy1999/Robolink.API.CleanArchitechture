using MediatR;

namespace Robolink.Application.Commands.ProjectPhases
{
    public record RemovePhaseFromProjectCommand(Guid PhaseConfigId) : IRequest<bool>;
}