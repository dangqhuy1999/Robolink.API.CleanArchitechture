using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.SystemPhases
{
    public record CreateSystemPhaseCommand(
        CreateSystemPhaseRequest Request,
        string? CreatedBy = null
    ) : IRequest<SystemPhaseDto>;
}