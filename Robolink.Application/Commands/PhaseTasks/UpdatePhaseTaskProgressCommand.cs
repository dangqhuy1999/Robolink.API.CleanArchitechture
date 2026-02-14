using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.PhaseTasks
{
    public record UpdatePhaseTaskProgressCommand(
        Guid PhaseTaskId,
        int ProcessRate,  // 0-100%
        int Status        // Task_Status enum
    ) : IRequest<PhaseTaskDto>;
}