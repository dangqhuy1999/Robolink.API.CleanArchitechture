using MediatR;

namespace Robolink.Application.Commands.PhaseTasks
{
    public record DeletePhaseTaskCommand(Guid PhaseTaskId) : IRequest<bool>;
}