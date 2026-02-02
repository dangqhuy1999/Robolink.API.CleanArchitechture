using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Queries.ProjectPhases
{
    public record GetProjectPhasesQuery(Guid ProjectId) : IRequest<IEnumerable<ProjectPhaseConfigDto>>;
}