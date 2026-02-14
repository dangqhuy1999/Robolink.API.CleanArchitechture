using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.ProjectPhases
{
    public record GetProjectPhasesQuery(Guid ProjectId) : IRequest<IEnumerable<ProjectPhaseConfigDto>>;
}