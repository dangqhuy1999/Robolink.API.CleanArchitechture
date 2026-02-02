using MediatR;

namespace Robolink.Application.Commands.SystemPhases
{
    public record DeleteSystemPhaseCommand(Guid SystemPhaseId) : IRequest<bool>;
}