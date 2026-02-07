using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Commands.SystemPhases
{
    public record CreateSystemPhaseCommand(
        string Name,
        string? Description,
        int DefaultSequence,
        bool IsActive = true,
        string? CreatedBy = null
    ) : IRequest<SystemPhaseDto>;
}