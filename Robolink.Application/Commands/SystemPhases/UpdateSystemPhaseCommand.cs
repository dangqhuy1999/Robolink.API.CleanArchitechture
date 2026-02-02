using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Commands.SystemPhases
{
    public record UpdateSystemPhaseCommand(
        Guid SystemPhaseId,
        string Name,
        string? Description,
        bool IsActive
    ) : IRequest<SystemPhaseDto>;
}