using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Queries.SystemPhases
{
    public record GetAllSystemPhasesQuery : IRequest<IEnumerable<SystemPhaseDto>>;
}