using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Queries.PhaseTasks
{
    // Simple query contract — adjust parameters as your handlers expect
    public record GetPhaseTasksQuery(Guid ProjectId, Guid ProjectSystemPhaseConfigId) : IRequest<IEnumerable<PhaseTaskDto>>;
}