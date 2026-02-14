using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.SystemPhases
{
    public record GetAllSystemPhasesQuery : IRequest<IEnumerable<SystemPhaseDto>>;
}