using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.SystemPhases
{
    public record UpdateSystemPhaseCommand(
        Guid SystemPhaseId,
        string Name,
        string? Description,
        int DefaultSequence,
        bool IsActive
    ) : IRequest<SystemPhaseDto>;
}