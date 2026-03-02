using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.SystemPhases
{
    public record UpdateSystemPhaseCommand(UpdateSystemPhaseRequest Request) : IRequest<SystemPhaseDto>;
}