using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Queries.PhaseTasks
{
    public record GetPhaseTaskByIdQuery(Guid PhaseTaskId) : IRequest<PhaseTaskDto>;
}