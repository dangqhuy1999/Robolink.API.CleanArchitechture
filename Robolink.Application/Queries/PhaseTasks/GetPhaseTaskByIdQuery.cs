using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.PhaseTasks
{
    public record GetPhaseTaskByIdQuery(Guid PhaseTaskId) : IRequest<PhaseTaskDto>;
}