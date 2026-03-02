using MediatR;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.PhaseTasks
{
    public class GetPhaseTaskByIdQuery : IRequest<PhaseTaskDto?>
    {
        public Guid PhaseTaskId { get; set; }

        public GetPhaseTaskByIdQuery(Guid phaseTaskId)
        {
            PhaseTaskId = phaseTaskId;
        }
    }
}